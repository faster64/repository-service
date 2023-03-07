using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using ServiceStack;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region GET classes
    [Route("/holidays",
        "GET",
        Summary = "Lấy danh sách kỳ nghỉ / lễ tết",
        Notes = "")
    ]
    public class GetListHolidayReq : QueryDb<Holiday>, IReturn<object>
    {
        [QueryDbField(Template = "(Name LIKE {Value})", Field = "Name", ValueFormat = "%{0}%")]
        public string Keyword { get; set; }
        public bool HasSummaryRow { get; set; }
    }

    [Route("/holidays/{Id}",
        "GET",
        Summary = "Lấy thông tin chi tiết kỳ nghỉ / lễ tết theo Id",
        Notes = "")
    ]
    public class GetHolidayByIdReq : IReturn<object>
    {
        public long Id { get; set; }
    }
    #endregion

    #region POST classes
    [Route("/holidays",
        "POST",
        Summary = "Tạo mới kỳ nghỉ / lễ tết",
        Notes = "")
    ]
    public class CreateHolidayReq : IReturn<object>
    {
        /// <summary>
        /// Kì nghỉ tạo mới
        /// </summary>
        public HolidayDto Holiday { get; set; }

        /// <summary>
        /// Nếu true, tạo mới kỳ nghỉ lễ tết đồng thời thực hiện hủy bỏ các chi tiết làm việc thuộc kì nghỉ lễ tết
        /// Nếu false, tạo mới kỳ nghỉ lễ tết và không hủy bỏ các chi tiết làm việc thuộc kì nghỉ lễ tết
        /// </summary>
        public bool IsCancelClocking { get; set; }

        public bool IsOverLapClocking { get; set; }
    }

    [Route("/holidays/create-national-holiday",
        "POST",
        Summary = "Tạo ngày nghỉ lễ quốc gia tự động",
        Notes = "")
    ]
    public class CreateNationalHolidayReq : IReturn<object>
    {

    }
    #endregion

    #region PUT classes
    [Route("/holidays/{Id}",
        "PUT",
        Summary = "Cập nhật kỳ nghỉ / lễ tết",
        Notes = "")
    ]
    public class UpdateHolidayReq : IReturn<object>
    {
        public long Id { get; set; }
        public HolidayDto Holiday { get; set; }
        public bool? IsAddClocking { get; set; }
        public bool? IsCancelClocking { get; set; }
        public bool IsOverLapClocking { get; set; }
    }
    #endregion

    #region DELETE classes
    [Route("/holidays/{Id}",
        "DELETE",
        Summary = "Xóa kỳ nghỉ / lễ tết",
        Notes = "")
    ]
    public class DeleteHolidayReq : IReturn<object>
    {
        public long Id { get; set; }
        public bool? IsAddClocking { get; set; }
        public bool IsOverLapClocking { get; set; }
    }
    #endregion
}
