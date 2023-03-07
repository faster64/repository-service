using KiotVietTimeSheet.Application.Dto;
using ServiceStack;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/allowance",
        "POST",
        Summary = "Tạo phụ cấp",
        Notes = "")
    ]
    public class CreateAllowanceReq
    {
        public AllowanceDto Allowance { get; set; }
    }

    [Route("/allowance/{Id}",
        "PUT",
        Summary = "Cập nhật phụ cấp",
        Notes = "")
    ]
    public class UpdateAllowanceReq
    {
        public long Id { get; set; }
        public AllowanceDto Allowance { get; set; }
    }

    [Route("/allowance/{Id}",
        "DELETE",
        Summary = "Xóa phụ cấp",
        Notes = "")
    ]
    public class DeleteAllowanceReq
    {
        public long Id { get; set; }
    }

    [Route("/allowance",
        "GET",
        Summary = "Lấy danh sách phụ cấp",
        Notes = "")
    ]
    public class GetListAllowanceReq : QueryDb<Allowance>, IReturn<object>
    {
    }

    [Route("/allowance/list-by-ids",
        "GET",
        Summary = "Lấy phụ cấp theo danh sách id",
        Notes = "")
    ]
    public class GetListAllowanceByIdsReq : QueryDb<Allowance>, IReturn<object>
    {
        [QueryDbField(Field = "Id", Template = "{Field} IN ({Values})")]
        public List<long> Ids { get; set; }
    }

    [Route("/allowance/{Id}",
        "GET",
        Summary = "Lấy phụ cấp theo id",
        Notes = "")
    ]
    public class GetAllowanceByIdReq : IReturn<object>
    {
        public long Id { get; set; }
    }

}
