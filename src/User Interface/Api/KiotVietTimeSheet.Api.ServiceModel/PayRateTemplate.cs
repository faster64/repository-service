using KiotVietTimeSheet.Application.Dto;
using ServiceStack;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/pay-rate-template",
        "GET",
        Summary = "Lấy danh sách loại lương",
        Notes = "")
    ]
    public class GetPayRateTemplateReq : QueryDb<PayRateTemplate>, IReturn<object>
    {
        [QueryDbField(Template = "(BranchId = {Value})", Field = "BranchId", ValueFormat = "{0}")]
        public int? BranchId { get; set; }
        [QueryDbField(Template = "Status IN ({Values})", Field = "Status")]
        public int[] Statuses { get; set; }

    }

    [Route("/pay-rate-template/{Id}",
           "GET",
           Summary = "Lấy thông tin mức lương theo id truyền vào",
           Notes = "")
       ]
    public class GetPayRateTemplateByIdReq : QueryDb<PayRateTemplate>, IReturn<object>
    {
        public long Id { get; set; }
    }

    [Route("/pay-rate-template",
           "POST",
           Summary = "Tạo mới một loại lương",
           Notes = "")
       ]
    public class CreatePayRateTemplateReq : IReturn<object>
    {
        public PayRateFormDto PayRateTemplate { get; set; }
        public int BranchId { get; set; }
        public bool IsGeneralSetting { get; set; }
    }

    [Route("/pay-rate-template/{id}",
           "PUT",
           Summary = "Cập nhật loại lương",
           Notes = "")
       ]
    public class UpdatePayRateTemplateReq : IReturn<object>
    {
        public long Id { get; set; }
        public PayRateFormDto PayRateTemplate { get; set; }
        public bool UpdatePayRate { get; set; }
        public bool IsGeneralSetting { get; set; }
    }

    [Route("/pay-rate-template/{id}",
        "DELETE",
        Summary = "Xóa loại lương",
        Notes = "")
    ]
    public class DeletePayRateTemplateReq : IReturn<object>
    {
        public long Id { get; set; }
        public bool IsGeneralSetting { get; set; }
    }

    [Route("/pay-rate-template/copy",
        "POST",
        Summary = "Copy loại lương",
        Notes = "")
    ]
    public class CopyPayRateTemplateReq : IReturn<object>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
