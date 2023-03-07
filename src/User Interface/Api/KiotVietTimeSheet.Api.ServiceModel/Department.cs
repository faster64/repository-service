using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region GET classes
    [Route("/employees/department",
        "GET",
        Summary = "Lấy danh sách phòng ban",
        Notes = "")
    ]
    public class GetListDepartmentReq : QueryDb<Department>, IReturn<object>
    {
        [QueryDbField(Template = "(Name LIKE {Value})", Field = "Name", ValueFormat = "%{0}%")]
        public string Keyword { get; set; }

        public bool? IsActive { get; set; }
    }
    #endregion

    #region POST classes
    [Route("/employees/department",
        "POST",
        Summary = "Tạo mới phòng ban",
        Notes = "")
    ]
    public class CreateDepartmentReq : IReturn<object>
    {
        public DepartmentDto Department { get; set; }
    }
    #endregion

    #region PUT classes
    [Route("/employees/department/{Id}",
        "PUT",
        Summary = "Cập nhật phòng ban",
        Notes = "")
    ]
    public class UpdateDepartmentReq : IReturn<object>
    {
        public long Id { get; set; }
        public DepartmentDto Department { get; set; }
    }
    #endregion

    #region DELETE classes
    [Route("/employees/department/{Id}",
        "DELETE",
        Summary = "Xóa phòng ban",
        Notes = "")
    ]
    public class DeleteDepartmentReq : IReturn<object>
    {
        public long Id { get; set; }
    }
    #endregion
}
