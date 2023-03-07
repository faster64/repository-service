using System.Collections.Generic;
using KiotVietTimeSheet.Api.ServiceModel.Types;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region GET classes

    [Route("/commission-details",
        "GET",
        Summary = "Lấy danh sách bảng hoa hồng",
        Notes = "")
    ]
    public class GetCommissionDetailsReq : PagingBaseReq, IReturn<object>
    {
        public List<long> CommissionIds { get; set; }
        public int? ProductCategoryId { get; set; }
        public string ProductCodeKeyword { get; set; }
        public string ProductNameKeyword { get; set; }
        public bool? IncludeInActive { get; set; }
    }

    [Route("/commission-details/get-by-product-id",
        "GET",
        Summary = "Lấy danh sách bảng hoa hồng",
        Notes = "")
    ]
    public class GetCommissionDetailsProductIdReq : QueryDb<CommissionDetail>, IReturn<object>
    {
        [QueryDbField(Template = "({Field} IN {Value})", Field = "CommissionId")]
        public List<long> CommissionIdIn { get; set; }
        [QueryDbField(Template = "({Field} = {Value})", Field = "ObjectId")]
        public long ProductId { get; set; }
    }

 

    [Route("/commission-details/get-all-by-commission-ids",
        "GET",
        Summary = "Lấy danh sách chi tiết bảng hoa hồng theo id bảng hoa hồng",
        Notes = "")
    ]
    public class GetAllCommissionDetailsByCommissionIdsReq : IReturn<object>
    {
        public List<long> CommissionIds { get; set; }
    }

    [Route("/commission-details/insert-commission-detail-status",
        "GET",
        Summary = "Kiểm tra trạng thái thêm mới CommissionDetail",
        Notes = "")
    ]

    public class GetInsertCommissionDetailStatus : IReturn<object> { }
    #endregion

    #region Post classes

    [Route("/commission-details/get-by-commission-ids",
        "POST",
        Summary = "Lấy danh sách chi tiết bảng hoa hồng theo id bảng hoa hồng",
        Notes = "")
    ]
    public class GetCommissionDetailsByCommissionIdsReq : IReturn<object>
    {
        public List<long> CommissionIds { get; set; }
        public List<long> ProductIds { get; set; }
    }

    [Route("/commission-details",
        "POST",
        Summary = "Tạo chi tiết hoa hồng theo từng sản phẩm",
        Notes = "")
    ]

    public class CreateCommissionDetailReq : IReturn<object>
    {
        public List<CommissionDetailDto> CommissionDetails { get; set; }

        public bool IsNotAudit { get; set; }
    }

    [Route("/commission-details/create-by-category",
        "POST",
        Summary = "Tạo chi tiết hoa hồng theo nhóm hàng",
        Notes = "")
    ]
    public class CreateCommissionDetailByProductCategoryReq : IReturn<object>
    {
        public List<long> CommissionIds { get; set; }
        public ProductCategoryReqDto ProductCategory { get; set; }
    }

    [Route("/commission-details/create-by-category-async",
        "POST",
        Summary = "Tạo chi tiết hoa hồng theo nhóm hàng, Thực hiện bất đồng bộ",
        Notes = "")
    ]
    public class CreateCommissionDetailByProductCategoryAsyncReq : IReturn<object>
    {
        public List<long> CommissionIds { get; set; }
        public ProductCategoryReqDto ProductCategory { get; set; }
    }

    [Route("/commission-details/create-by-product",
        "POST",
        Summary = "Tạo chi tiết hoa hồng theo sản phẩm",
        Notes = "")
    ]
    public class CreateCommissionDetailByProductReq : IReturn<object>
    {
        public List<long> CommissionIds { get; set; }
        public ProductCommissionDetailDto Product { get; set; }
    }

    [Route("/commission-details/create-categories",
        "POST",
        Summary = "Tạo chi tiết hoa hồng là nhóm hàng",
        Notes = "")
    ]
    public class CreateCommissionDetailCategoryIdsReq : IReturn<object>
    {
        public List<long> CommissionIds { get; set; }
        public List<int> CategoryIds { get; set; }
    }

    [Route("/commission-details/create-multiple-commission-detail",
        "POST",
        Summary = "Tạo nhiều chi tiết hoa hồng",
        Notes = "")
    ]
    public class CreateMultipleCommissionDetailsReq : IReturn<object>
    {
        public ProductCommissionDetailDto Product { get; set; }
        public List<long> TotalCommissionIds { get; set; }
        public decimal? Value { get; set; }
        public decimal? ValueRatio { get; set; }
        public bool IsUpdateForAllCommission { get; set; }
        public int CategoryId { get; set; }
        public string ProductCodeKeyword { get; set; }
        public string ProductNameKeyword { get; set; }
        public CategoryCommissionDetailDto Category { get; set; }
    }

    #endregion

    #region DELETE classes

    [Route("/commission-details/delete",
        "PUT",
        Summary = "Xóa chi tiết hoa hồng",
        Notes = "")
    ]
    public class DeleteCommissionDetailReq : IReturn<object>
    {
        public List<long> CommissionIds { get; set; }
        public List<ProductCommissionDetailDto> Products { get; set; }
        public List<long> CategoryIds { get; set; }
    }

    #endregion

    #region PUT classes

    [Route("/commission-details/update-value",
        "PUT",
        Summary = "Cập nhật giá trị mức hoa hồng mỗi sản phẩm bán ra",
        Notes = "")
    ]
    public class UpdateValueOfCommisionDetailReq : IReturn<object>
    {
        public ProductCommissionDetailDto Product { get; set; }
        public List<long> TotalCommissionIds { get; set; }
        public decimal? Value { get; set; }
        public decimal? ValueRatio { get; set; }
        public bool IsUpdateForAllCommission { get; set; }
        public int CategoryId { get; set; }
        public string ProductCodeKeyword { get; set; }
        public string ProductNameKeyword { get; set; }
        public CategoryCommissionDetailDto Category { get; set; }
    }

    [Route("/commission-details",
        "PUT",
        Summary = "Cập nhật chi tiết hoa hồng",
        Notes = "")
    ]
    public class UpdateCommissionDetailReq : IReturn<object>
    {
        public List<CommissionDetailDto> Commissiondetails { get; set; }
    }

    #endregion
}
