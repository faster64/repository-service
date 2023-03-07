using KiotVietTimeSheet.Application.Dto;
using ServiceStack;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/deduction",
        "POST",
        Summary = "Tạo giảm trừ",
        Notes = "")
    ]
    public class CreateDeductionReq
    {
        public DeductionDto Deduction { get; set; }
        public DeductionRuleValueDetail DeductionDetail { get; set; }
    }

    [Route("/deduction/{Id}",
        "PUT",
        Summary = "Cập nhật giảm trừ",
        Notes = "")
    ]
    public class UpdateDeductionReq
    {
        public long Id { get; set; }
        public DeductionDto Deduction { get; set; }
        public DeductionRuleValueDetail DeductionDetail { get; set; }
    }

    [Route("/deduction/{Id}",
        "DELETE",
        Summary = "Xóa giảm trừ",
        Notes = "")
    ]
    public class DeleteDeductionReq
    {
        public long Id { get; set; }
    }

    [Route("/deduction",
       "GET",
       Summary = "Lấy danh sách giảm trừ",
       Notes = "")
   ]
    public class GetListDeductionReq : QueryDb<Deduction>, IReturn<object>
    {
    }

    [Route("/deduction/list-by-ids",
        "GET",
        Summary = "Lấy giảm trừ theo danh sách id",
        Notes = "")
    ]
    public class GetListDeductionByIdsReq : QueryDb<Deduction>, IReturn<object>
    {
        [QueryDbField(Field = "Id", Template = "{Field} IN ({Values})")]
        public List<long> Ids { get; set; }
    }

    [Route("/deduction/{Id}",
        "GET",
        Summary = "Lấy giảm trừ theo id",
        Notes = "")
    ]
    public class GetDeductionByIdReq : IReturn<object>
    {
        public long Id { get; set; }

    }
}
