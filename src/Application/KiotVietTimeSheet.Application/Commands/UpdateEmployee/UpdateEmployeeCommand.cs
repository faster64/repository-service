using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateEmployee
{
    [RequiredPermission(TimeSheetPermission.Employee_Update)]
    public class UpdateEmployeeCommand : BaseCommand<EmployeeDto>
    {
        public EmployeeDto Employee { get; set; }
        public PayRateDto PayRate { get; set; }
        public int BlockUnit { get; }
        public UpdateEmployeeCommand(EmployeeDto employee, PayRateDto payRate, int blockUnit)
        {
            Employee = employee;
            PayRate = payRate;
            BlockUnit = blockUnit;
        }
    }
}
