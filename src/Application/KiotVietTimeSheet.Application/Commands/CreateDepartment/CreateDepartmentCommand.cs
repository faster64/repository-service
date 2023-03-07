using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateDepartment
{
    [RequiredPermission(TimeSheetPermission.Department_Create)]
    public class CreateDepartmentCommand : BaseCommand<DepartmentDto>
    {
        public DepartmentDto Department { get; set; }

        public CreateDepartmentCommand(DepartmentDto department)
        {
            Department = department;
        }
    }
}
