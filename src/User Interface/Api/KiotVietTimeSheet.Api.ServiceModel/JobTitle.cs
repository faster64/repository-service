using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region GET classes
    [Route("/employees/job-title",
        "GET",
        Summary = "Lấy danh sách chức danh",
        Notes = "")
    ]
    public class GetListJobTitleReq : QueryDb<JobTitle>, IReturn<object>
    {
        [QueryDbField(Template = "(Name LIKE {Value})", Field = "Name", ValueFormat = "%{0}%")]
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
    #endregion

    #region POST classes
    [Route("/employees/job-title",
        "POST",
        Summary = "Tạo mới chức danh",
        Notes = "")
    ]
    public class CreateJobTitleReq : IReturn<object>
    {
        public JobTitleDto JobTitle { get; set; }
    }
    #endregion

    #region PUT classes
    [Route("/employees/job-title/{Id}",
        "PUT",
        Summary = "Cập nhật chức danh",
        Notes = "")
    ]
    public class UpdateJobTitleReq : IReturn<object>
    {
        public long Id { get; set; }
        public JobTitleDto JobTitle { get; set; }
    }
    #endregion

    #region DELETE classes
    [Route("/employees/job-title/{Id}",
        "DELETE",
        Summary = "Xóa chức danh",
        Notes = "")
    ]
    public class DeleteJobTitleReq : IReturn<object>
    {
        public long Id { get; set; }
    }
    #endregion
}
