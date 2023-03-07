using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region GET classes
    [Route("/commission",
        "GET",
        Summary = "Lấy danh sách bảng hoa hồng",
        Notes = "")
    ]
    public class GetListCommissionReq : QueryDb<Commission>, IReturn<object>
    {
        [QueryDbField(Template = "(Name LIKE {Value})", Field = "Name", ValueFormat = "%{0}%")]
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }

    [Route("/commission/current-branch",
        "GET",
        Summary = "Lấy danh sách bảng hoa hồng áp dụng với chi nhánh hiện tại",
        Notes = "")
    ]
    public class GetListCommissionCurrentBranchReq : QueryDb<Commission>, IReturn<object>
    {
        public bool IncludeDeleted { get; set; }
        public bool IncludeInActive { get; set; }
        public List<long> IncludeCommissionIds { get; set; }
    }

    [Route("/commission/has-any",
        "GET",
        Summary = "Kiểm tra xem trong shop của bất kỳ bảng hoa hồng nào không, chưa xóa",
        Notes = "")
    ]
    public class GetHasAnyCommissionReq : QueryDb<Commission>, IReturn<bool>
    {
        public bool IncludeDeleted { get; set; }
    }

    [Route("/commission/list-by-ids",
        "GET",
        Summary = "Lấy danh sách bảng hoa hồng theo danh sách id",
        Notes = "")
    ]
    public class GetListCommissionByIdsReq : QueryDb<Commission>, IReturn<object>
    {
        public List<long> Ids { get; set; }
        public bool CheckInActive { get; set; }
    }

    [Route("/commission/get-commisison-table-by-id",
        "GET",
        Summary = "Lấy bảng hoa hông theo id",
        Notes = "")
    ]
    public class GetCommissionTableByIdReq : QueryDb<Commission>, IReturn<object>
    {
        public long Id { get; set; }
    }

    [Route("/commission/list-by-names",
        "GET",
        Summary = "Lấy danh sách bảng hoa hồng theo danh sách theo tên bảng hoa hồng",
        Notes = "")
    ]
    public class GetCommissionByNamesReq : QueryDb<Commission>, IReturn<object>
    {
        public List<string> Names { get; set; }
    }
    #endregion

    #region POST classes
    [Route("/commission",
        "POST",
        Summary = "Tạo mới bảng hoa hồng",
        Notes = "")
    ]
    public class CreateCommissionReq : IReturn<object>
    {
        public CommissionDto Commission { get; set; }
    }
    #endregion

    #region PUT classes
    [Route("/commission",
        "PUT",
        Summary = "Cập nhật bảng hoa hồng",
        Notes = "")
    ]
    public class UpdateCommissionReq : IReturn<object>
    {
        public CommissionDto Commission { get; set; }
    }
    #endregion

    #region DELETE classes
    [Route("/commission/{Id}",
        "DELETE",
        Summary = "Xóa hoa hồng",
        Notes = "")
    ]
    public class DeleteCommissionReq : IReturn<object>
    {
        public long Id { get; set; }
    }
    #endregion
}
