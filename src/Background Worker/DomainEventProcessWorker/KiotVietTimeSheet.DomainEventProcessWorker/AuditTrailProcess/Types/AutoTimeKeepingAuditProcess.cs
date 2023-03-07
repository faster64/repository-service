using System;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.AutoTimeKeepingEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types
{
    public class AutoTimeKeepingAuditProcess : BaseAuditProcess
    {
        private readonly IAppSettings _appSettings;
        private EfDbContext _db;
        private readonly IKiotVietApiClient _kiotVietApiClient;
        private readonly ICacheClient _cacheClient;
        private readonly Helper _helper = new Helper();
        public AutoTimeKeepingAuditProcess(
                IKiotVietApiClient kiotVietApiClient,
                IAuditProcessFailEventService auditProcessFailEventService,
                IAppSettings appSettings, ICacheClient cacheClient
            ) : base(kiotVietApiClient, auditProcessFailEventService)
        {
            _kiotVietApiClient = kiotVietApiClient;
            _appSettings = appSettings;
            _cacheClient = cacheClient;
        }

        public async Task WriteAutoTimekeepingAudit(AutoTimeKeepingIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                        @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var autoTimeKeepingResults = @event.FingerPrintLogs
                       .Where(log => !log.IsExist)
                       .ToList();

                        var fingerMachineIds = autoTimeKeepingResults
                            .Select(x => x.FingerPrintLog.MachineId)
                            .Distinct()
                            .ToList();

                        foreach (var machineId in fingerMachineIds)
                        {
                            var machine = await _db.FingerMachine
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x =>
                                    x.TenantId == @event.Context.TenantId &&
                                    x.BranchId == @event.Context.BranchId &&
                                    x.MachineId == machineId
                                );

                            if (machine == null) continue;

                            var autoTimeKeepingResultForMachineIds = autoTimeKeepingResults
                                .Where(x => x.FingerPrintLog.MachineId == machineId)
                                .ToList();

                            var branch = await KiotVietApiClient.GetBranchById(machine.BranchId, @event.Context.TenantId, @event.Context.GroupId, @event.Context.RetailerCode);
                            var branchName = branch?.Name ?? string.Empty;

                            var subContent = string.Join("",
                                autoTimeKeepingResultForMachineIds
                                    .Select(async x => await RenderAutoTimekeepingAudit(x, @event.Context))
                                    .Select(x => x.Result)
                                    .ToList());

                            var content = $"Đồng bộ dữ liệu chấm công vân tay từ {machine.MachineName ?? ""} lên {branchName}. Bao gồm: {subContent}";
                            var auditLog = GenerateLog(
                                TimeSheetFunctionTypes.AutoTimeKeeping,
                                TimeSheetAuditTrailAction.AutoTimekeeping,
                                content,
                                @event.Context
                            );
                            await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        private async Task<string> RenderAutoTimekeepingAudit(AutoTimeKeepingResult timeKeepingResult, IntegrationEventContext context)
        {
            var fingerPrintLog = timeKeepingResult.FingerPrintLog;
            var employeeId = (await _db.FingerPrint.AsNoTracking()
                                 .FirstOrDefaultAsync(f =>
                                    f.TenantId == context.TenantId &&
                                    f.BranchId == context.BranchId &&
                                    f.FingerCode == fingerPrintLog.FingerCode
                                )
                             )?.EmployeeId ?? 0;

            var employee = await _db.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.Id == employeeId &&
                    e.TenantId == context.TenantId &&
                    e.BranchId == context.BranchId
                );

            var content = $"</br>- {fingerPrintLog.CheckDateTime:dd/MM/yyyy HH:mm} - " +
                          $"{fingerPrintLog.FingerCode} - " +
                          $"{employee?.Name ?? ""}: " +
                          $"{(timeKeepingResult.IsSuccess ? "Thành công" : "KHÔNG thành công")}" +
                          $"{(timeKeepingResult.IsSuccess ? "" : $", Lí do: {timeKeepingResult.Message}")}";

            return content;
        }
    }
}
