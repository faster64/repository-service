using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using ServiceStack;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region get method
    [Route("/gpsinfos",
        "GET",
        Summary = "Lấy danh sách GPS",
        Notes = "")
    ]
    public class GetListGpsInfoReq : QueryDb<GpsInfo>, IReturn<object>
    {
        public bool WithDeleted { get; set; }
        [QueryDbField(Field = "TenantId", Template = "{Field} IN ({Values})")]
        public List<long> TenantIds { get; set; }
    }

    [Route("/gpsinfos/{Id}",
       "GET",
       Summary = "Lấy chi tiết GPS theo Id",
       Notes = "")
   ]
    public class GetGpsInfoByIdReq : QueryDb<GpsInfo>, IReturn<object>
    {
        public long Id { get; set; }
    }
    [Route("/gprs/connect-total", "GET", Summary = "Tổng số kết nối GPS", Notes = "")]
    public class GetTotalGprsReq : IReturn<object>
    {

    }

    #endregion

    #region post method
    [Route("/gpsinfos",
        "POST",
        Summary = "Tạo mới Gps info",
        Notes = "")
    ]
    public class CreateGpsInfoReq : IReturn<object>
    {
        public GpsInfoDto GpsInfo { get; set; }

    }
    #endregion

    #region put method
    [Route("/gpsinfos/{Id}",
       "PUT",
       Summary = "Cập nhật GPS",
       Notes = "")
   ]
    public class UpdateGpsInfoReq :  IReturn<object>
    {
        public long Id { get; set; }
        public GpsInfoDto GpsInfo { get; set; }
     
    }
    [Route("/gpsinfos/changeQrKey/{Id}",
       "PUT",
       Summary = "Cập nhật GPS",
       Notes = "")
   ]
    public class ChangeQrKeyGpsInfoReq : IReturn<object>
    {
        public long Id { get; set; }      

    }
    #endregion

    #region delete method
    [Route("/gpsinfos/{Id}",
        "DELETE",
        Summary = "Xóa  Gps infor",
        Notes = "")
    ]
    public class DeleteGpsInfoReq : IReturn<object>
    {
        public long Id { get; set; }
    }
    #endregion
}
