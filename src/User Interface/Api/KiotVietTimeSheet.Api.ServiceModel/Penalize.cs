using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/penalizes",
            "GET",
            Summary = "Lấy danh sách giảm trừ",
            Notes = "")
    ]
    public class GetAllPenalizeReq : QueryDb<Penalize>, IReturn<object>
    {

    }

    [Route("/penalizes",
        "POST",
        Summary = "Tạo vi phạm",
        Notes = "")
    ]
    public class CreatePenalizesReq
    {
        public PenalizeDto Penalize { get; set; }
    }

    [Route("/penalizes/{Id}",
        "PUT",
        Summary = "Cập nhật vi phạm",
        Notes = "")
    ]
    public class UpdatePenalizesReq
    {
        public long Id { get; set; }
        public PenalizeDto Penalize { get; set; }
    }

    [Route("/penalizes/{Id}",
        "DELETE",
        Summary = "Xóa vi phạm",
        Notes = "")
    ]
    public class DeletePenalizesReq
    {
        public long Id { get; set; }
    }
}
