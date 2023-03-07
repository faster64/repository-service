using KiotVietTimeSheet.Application.EventBus.Events.PayslipEvents;
using System.Threading.Tasks;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
using MediatR;
using KiotVietTimeSheet.Application.Queries.GetPaysheetById;
using KiotVietTimeSheet.Application.Queries.GetEmployeeById;
using KiotVietTimeSheet.Application.ServiceClients;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class PayslipAuditProcess : BaseAuditProcess
    {
        private readonly IMediator _mediator;

        public PayslipAuditProcess(
            IKiotVietInternalService kiotVietInternalService,
            IMediator mediator
        ) : base(kiotVietInternalService)
        {
            _mediator = mediator;
        }

        public async Task WriteCancelPayslipLogAsync(CancelPayslipIntegrationEvent @event)
        {
            var paysheet = await _mediator.Send(new GetPaysheetByIdQuery(@event.Payslip.PaysheetId));
            var paysheetCode = paysheet != null ? paysheet.Code : string.Empty;
            var employee = await _mediator.Send(new GetEmployeeByIdQuery(@event.Payslip.EmployeeId));
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
            await AddLogAsync(auditLog);
        }
    }
}
