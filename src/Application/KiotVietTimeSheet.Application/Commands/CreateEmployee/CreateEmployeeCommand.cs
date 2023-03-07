using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateEmployee
{
    [RequiredPermission(TimeSheetPermission.Employee_Create)]
    public class CreateEmployeeCommand : BaseCommand<EmployeeDto>
    {
        public EmployeeDto EmployeeDto { get; set; }
        public PayRateDto PayRateDto { get; set; }
        public int? TypeInsert { get; }
        public int BlockUnit { get; }
        public CreateEmployeeCommand(EmployeeDto employeeDto, PayRateDto payRateDto, int? typeInsert, int blockUnit)
        {
            EmployeeDto = employeeDto;
            PayRateDto = payRateDto;
            TypeInsert = typeInsert;
            BlockUnit = blockUnit;
        }
    }

    public class CreateSyncEmployeeCommand : BaseCommand<SyncEmployeeDto>, IInternalRequest
    {
        public SyncEmployeeDto SyncEmployeeDto { get; set; }
        public int RetailerId { get; }
        public string RetailerCode { get; }
        public int BlockUnit { get; }

        public CreateSyncEmployeeCommand(SyncEmployeeDto employeeDto, int retailerId, string retailCode, int blockUnit)
        {
            SyncEmployeeDto = employeeDto;
            RetailerId = retailerId;
            RetailerCode = retailCode;
            BlockUnit = blockUnit;
        }
    }
}
