using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{

    [Route("/finger-machines",
        "GET",
        Summary = "Lấy danh sách máy chấm công",
        Notes = "")
    ]
    public class GetListFingerMachineReq : QueryDb<FingerMachine>, IReturn<object>
    {
        public bool WithDeleted { get; set; }
    }

    [Route("/finger-machines",
        "POST",
        Summary = "Thêm máy chấm công",
        Notes = "")
    ]
    public class CreateFingerMachineReq
    {
        public FingerMachineDto FingerMachine { get; set; }
    }

    [Route("/finger-machines",
        "DELETE",
        Summary = "Xóa máy chấm công",
        Notes = "")
    ]
    public class DeleteFingerMachineReq
    {
        public long Id { get; set; }
    }

    [Route("/finger-machines",
        "PUT",
        Summary = "Cập nhật máy chấm công",
        Notes = "")
    ]
    public class UpdateFingerMachineReq
    {
        public FingerMachineDto FingerMachine { get; set; }
    }
}
