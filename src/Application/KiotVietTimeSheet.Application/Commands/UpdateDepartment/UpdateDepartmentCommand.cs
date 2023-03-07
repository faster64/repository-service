using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateDepartment
{
    [RequiredPermission(TimeSheetPermission.Department_Update)]
    public class UpdateDepartmentCommand : BaseCommand
    {
        public DepartmentDto Department { get; set; }

        public UpdateDepartmentCommand(DepartmentDto department)
        {
            Department = department;
        }
    }
}
