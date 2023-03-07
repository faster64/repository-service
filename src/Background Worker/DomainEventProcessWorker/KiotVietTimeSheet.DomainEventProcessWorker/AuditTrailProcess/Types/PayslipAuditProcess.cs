using KiotVietTimeSheet.Application.EventBus.Events.PayslipEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types
{
    public class PayslipAuditProcess : BaseAuditProcess
    {
        private readonly IAppSettings _appSettings;
        private EfDbContext _db;
        private readonly IKiotVietApiClient _kiotVietApiClient;
        private readonly ICacheClient _cacheClient;
        private readonly Helper _helper = new Helper();
        public PayslipAuditProcess(
            IKiotVietApiClient kiotVietApiClient, 
            IAuditProcessFailEventService auditProcessFailEventService, 
            IAppSettings appSettings, 
            ICacheClient cacheClient
        ) : base(kiotVietApiClient, auditProcessFailEventService)
        {
            _kiotVietApiClient = kiotVietApiClient;
            _appSettings = appSettings;
            _cacheClient = cacheClient;
        }

        public async Task WriteCancelPayslipLogAsync(CancelPayslipIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                        @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var paysheet = await _db.Paysheet.FindAsync(@event.Payslip.PaysheetId);
                        var paysheetCode = paysheet != null ? paysheet.Code : string.Empty;

                        var employee = await _db.Employees.FindAsync(@event.Payslip.EmployeeId);
                        var employeeName = employee != null ? employee.Name : string.Empty;
                        var employeeCode = employee != null ? employee.Code : string.Empty;

                        var log = $"Hủy Phiếu lương: {@event.Payslip.Code}, " +
                                  $"Mã bảng lương: [PaysheetCode]{paysheetCode}[/PaysheetCode], " +
                                  $"Mã nhân viên: [EmployeeCode]{employeeCode}[/EmployeeCode], " +
                                  $"Tên nhân viên: {employeeName}";
                        var auditLog = GenerateLog(
                            TimeSheetFunctionTypes.PayslipManagement,
                            TimeSheetAuditTrailAction.Reject,
                            log,
                            @event.Context
                        );
                        await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                    }
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }
    }
}
