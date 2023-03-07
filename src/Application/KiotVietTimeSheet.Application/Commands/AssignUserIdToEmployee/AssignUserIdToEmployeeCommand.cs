using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.AssignUserIdToEmployee
{
    [RequiredPermission(TimeSheetPermission.Employee_Update)]
    public class AssignUserIdToEmployeeCommand : BaseCommand
    {
        public EmployeeDto Employee { get; set; }
        public int BlockUnit { get; }
        public AssignUserIdToEmployeeCommand(EmployeeDto employee, int blockUnit)
        {
            Employee = employee;
            BlockUnit = blockUnit;
        }
    }
}
