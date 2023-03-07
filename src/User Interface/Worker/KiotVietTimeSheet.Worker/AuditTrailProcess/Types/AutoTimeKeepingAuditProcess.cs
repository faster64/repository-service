using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.AutoTimeKeepingEvents;
using KiotVietTimeSheet.Application.Queries.GetEmployeeById;
using KiotVietTimeSheet.Application.Queries.GetFingerMachineByMachineId;
using KiotVietTimeSheet.Application.Queries.GetFingerPrintByFingerCode;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.SharedKernel.EventBus;
using MediatR;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class AutoTimeKeepingAuditProcess : BaseAuditProcess
    {
        private readonly IMediator _mediator;
        public AutoTimeKeepingAuditProcess(
                IKiotVietInternalService kiotVietInternalService,
                IMediator mediator
            ) : base(kiotVietInternalService)
        {
            _mediator = mediator;

        }

        public async Task WriteAutoTimekeepingAudit(AutoTimeKeepingIntegrationEvent @event)
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

                var machine = await _mediator.Send(new GetFingerMachineByMachineIdQuery(machineId));

                if (machine == null) continue;

                var autoTimeKeepingResultForMachineIds = autoTimeKeepingResults
                    .Where(x => x.FingerPrintLog.MachineId == machineId)
                    .ToList();
                int.TryParse(machine.BranchId.ToString(), out var brandId);
                var branch = await KiotVietInternalService.GetBranchByIdAsync(brandId, @event.Context.TenantId);
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
                await AddLogAsync(auditLog);
            }
        }

        private async Task<string> RenderAutoTimekeepingAudit(AutoTimeKeepingResult timeKeepingResult, IntegrationEventContext context)
        {
            var fingerPrintLog = timeKeepingResult.FingerPrintLog;
            var fingerPrint = await _mediator.Send( new GetFingerPrintByFingerCodeQuery(fingerPrintLog.FingerCode, context.BranchId));
            var employeeId = (fingerPrint)?.EmployeeId ?? 0;

            var employee = await _mediator.Send(new GetEmployeeByIdQuery(employeeId));

            var content = $"</br>- {fingerPrintLog.CheckDateTime:dd/MM/yyyy HH:mm} - " +
                          $"{fingerPrintLog.FingerCode} - " +
                          $"{employee?.Name ?? ""}: " +
                          $"{(timeKeepingResult.IsSuccess ? "Thành công" : "KHÔNG thành công")}" +
                          $"{(timeKeepingResult.IsSuccess ? "" : $", Lí do: {timeKeepingResult.Message}")}";

            return content;
        }
    }
}
