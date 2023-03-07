using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/finger-prints",
        "POST",
        Summary = "Thêm vân tay chấm công",
        Notes = "")
    ]
    public class CreateUpdateFingerPrintReq
    {
        public FingerPrintDto FingerPrint { get; set; }
    }

    [Route("/finger-prints",
        "GET",
        Summary = "Lấy danh sách vân tay chấm công",
        Notes = "")
    ]
    public class GetListFingerPrintReq : QueryDb<FingerPrint>, IReturn<object>
    {
        public bool WithDeleted { get; set; }
        public int? BranchId { get; set; }
        [QueryDbField(Field = "BranchId", Template = "{Field} IN ({Values})")]
        public List<int> BranchIds { get; set; }
    }


    [Route("/finger-prints/get-by-finger-code",
        "GET",
        Summary = "Lấy vân tay chấm công theo mã chấm công",
        Notes = "")
    ]
    public class GetFingerPrintByCodeReq : IReturn<object>
    {
        public string FingerCode { get; set; }
        public int BranchId { get; set; }
    }


    [Route("/finger-prints/get-by-employee",
        "GET",
        Summary = "Lấy vân tay chấm công theo nhân viên",
        Notes = "")
    ]
    public class GetFingerPrintByEmployeeReq : IReturn<object>
    {
        public long employeeId { get; set; }
        public long? branchId { get; set; }
        public string fingerCode { get; set; }
    }


    [Route("/finger-prints",
        "DELETE",
        Summary = "Xóa vân tay chấm công",
        Notes = "")
    ]
    public class DeleteFingerPrintReq
    {
        public long Id { get; set; }
    }
}
