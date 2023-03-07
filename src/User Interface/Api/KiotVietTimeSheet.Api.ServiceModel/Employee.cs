using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using ServiceStack;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using System.IO;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region GET classes
    [Route("/employees",
        "GET",
        Summary = "Lấy danh sách nhân viên",
        Notes = "")
    ]
    public class GetListEmployeeReq : QueryDb<Employee>, IReturn<object>
    {
        [QueryDbField(Template = "(Name LIKE {Value} OR Code Like {Value} OR MobilePhone Like {Value})", Field = "Name", ValueFormat = "%{0}%")]
        public string Keyword { get; set; }
        [QueryDbField(Template = "(MobilePhone LIKE {Value})", Field = "MobilePhone", ValueFormat = "%{0}%")]
        public string MobilePhone { get; set; }
        [QueryDbField(Template = "(Name LIKE {Value})", Field = "Name", ValueFormat = "%{0}%")]
        public string Name { get; set; }
        public int? BranchId { get; set; }
        public int?[] BranchIds { get; set; }
        [QueryDbField(Template = "ISNULL(DepartmentId, -1)  IN ({Values})", Field = "DepartmentId")]
        public long?[] DepartmentIds { get; set; }
        public long?[] JobTitleIds { get; set; }
        public bool WithDeleted { get; set; }
        [QueryDbField(Field = "UserId", Template = "{Field} IN ({Values})")]
        public List<long> UserIdIn { get; set; }
        [QueryDbField(Field = "Id", Template = "{Field} IN ({Values})")]
        public List<long> IdIn { get; set; }
        [QueryDbField(Template = "Id NOT IN ({Values})", Field = "Id")]
        public long?[] WithoutIds { get; set; }
        public bool? IsActive { get; set; }
        public bool IncludeFingerPrint { get; set; }
        public bool? FromManagementScreen { get; set; }
    }

    [Route("/employees/multiple-branch",
        "GET",
        Summary = "Lấy danh sách nhân viên trên nhiều chi nhánh",
        Notes = "")
    ]
    public class GetListEmployeeMultipleBranchReq : IReturn<object>
    {
        public List<int> BranchIds { get; set; }
        public List<long> ShiftIds { get; set; }
        public List<long?> DepartmentIds { get; set; }
        public bool? IsActive { get; set; }
        public bool? WithDeleted { get; set; }
        public string Keyword { get; set; }
        public List<long> EmployeeIds { get; set; }
    }

    [Route("/employees/for-paysheet",
        "GET",
        Summary = "Lấy danh sách nhân viên thêm vào một bảng lương",
        Notes = "")
    ]
    public class GetEmployeeForPaysheetReq : QueryDb<Employee>, IReturn<object>
    {
        public PaySheetWorkingPeriodStatuses SalaryPeriod { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        public int BranchId { get; set; }
    }

    [Route("/employees/{Id}",
        "GET",
        Summary = "Lấy chi tiết thông tin nhân viên theo Id",
        Notes = "")
    ]
    public class GetEmployeeByIdReq : QueryDb<Employee>, IReturn<object>
    {
        public long Id { get; set; }
    }

    [Route("/employees/get-by-branchid",
        "GET",
        Summary = "Lấy chi tiết thông tin nhân viên theo User Id",
        Notes = "")
    ]
    public class GetEmployeeByBranchId : IReturn<object>
    {
        public int BranchId { get; set; }
        public int TypeSearch { get; set; }
    }

    [Route("/employees/userid/{UserId}",
        "GET",
        Summary = "Lấy chi tiết thông tin nhân viên theo User Id",
        Notes = "")
    ]
    public class GetEmployeeByUserIdReq : QueryDb<Employee>, IReturn<object>
    {
        public long UserId { get; set; }
    }

    [Route("/employees/get-by-current-user",
        "GET",
        Summary = "Lấy chi tiết thông tin nhân viên theo User Id",
        Notes = "")
    ]
    public class GetEmployeeByCurrentUser : IReturn<object>
    {

    }

    [Route("/employees/get-available-employees",
        "GET",
        Summary = "Lấy ra danh sách các nhân viên có thể làm thay",
        Notes = "")
    ]
    public class GetAvailableEmployeesReq : QueryDb<Employee>, IReturn<object>
    {
        public int BranchId { get; set; }
        public long WithoutId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Keyword { get; set; }
    }

    [Route("/employees/check-assign-userid-to-employee/{employeeId}",
        "GET",
        Summary = "Cập nhật UserId cho nhân viên",
        Notes = "")
    ]
    public class CheckAssignUserIdToEmployeeReq : IReturn<object>
    {
        public long EmployeeId { get; set; }
        public long UserId { get; set; }
        public bool IsCreateUser { get; set; }
    }

    [Route("/employees/check-block-employee/",
        "GET",
        Summary = "Kiểm tra số lượng nhân viên Vươt quá giới hạn của cửa hàng",
        Notes = "")
    ]
    public class CheckTotalEmployeeWithBlockEmployee : IReturn<object>
    {

    }

    [Route("/employees/two-fa-pin",
        "GET",
        Summary = "Lấy mã PIN 2FA",
        Notes = "")
    ]
    public class GetTwoFaPinReq : IReturn<object>
    {
        public long EmployeeId { get; set; }
        public long? UserId { get; set; }
    }

    [Route("/employees/verify-two-fa-pin",
        Summary = "Xác nhận mã PIN 2FA",
        Notes = "")
    ]
    public class VerifyTwoFaPinReq : IReturn<object>
    {
        public long EmployeeId { get; set; }
        public string Pin { get; set; }
    }
    #endregion

    #region POST classes
    [Route("/employees",
        "POST",
        Summary = "Tạo mới nhân viên",
        Notes = "")
    ]
    public class CreateEmployeeReq : IRequiresRequestStream, IReturn<object>
    {
        public EmployeeDto Employee { get; set; }
        public PayRateDto PayRate { get; set; }
        public Stream RequestStream { get; set; }
        public int TypeInsert { get; set; }
    }

    [Route("/employees/delete-multiple",
        "POST",
        Summary = "Xóa nhiều nhân viên theo danh sách id truyền lên",
        Notes = "")
    ]
    public class DeleteMultipleEmployeeReq : IReturn<object>
    {
        public long[] EmployeeIds { get; set; }
    }

    [Route("/employees/delete-multiple-by-branch-id",
        "POST",
        Summary = "Xóa nhiều nhân viên theo id chi nhánh trả lương truyền lên",
        Notes = "")
    ]
    public class DeleteMultipleEmployeeByBranchIdReq : IReturn<object>
    {
        public long BranchId { get; set; }
    }

    [Route("/employees/unassign-user",
        "POST",
        Summary = "Xóa UserId cho các nhân viên theo danh sách user id",
        Notes = "")
    ]
    public class UnAssignUserWhenDeleteUserReq : IReturn<object>
    {
        public long UserId { get; set; }
    }
    #endregion

    #region PUT classes
    [Route("/employees/{Id}",
        "PUT",
        Summary = "Cập nhật nhân viên",
        Notes = "")
    ]
    public class UpdateEmployeeReq : IRequiresRequestStream, IReturn<object>
    {
        public long Id { get; set; }
        public EmployeeDto Employee { get; set; }
        public PayRateDto PayRate { get; set; }
        public Stream RequestStream { get; set; }
    }

    [Route("/employees/assign-userid-to-employee",
        "PUT",
        Summary = "Cập nhật UserId cho nhân viên",
        Notes = "")
    ]
    public class AssignUserIdToEmployeeReq : IReturn<object>
    {
        public EmployeeDto Employee { get; set; }
    }
    #endregion

    #region DELETE classes
    [Route("/employees/{Id}",
        "DELETE",
        Summary = "Xóa nhân viên",
        Notes = "")
    ]
    public class DeleteEmployeeReq : IReturn<object>
    {
        public long Id { get; set; }
    }

    [Route("/employees/employee-profile-picture/{Id}",
        "DELETE",
        Summary = "Xóa ảnh nhân viên",
        Notes = "")
    ]
    public class DeleteEmployeeProfilePictureReq : IReturn<object>
    {
        public long Id { get; set; }
    }
    #endregion
}
