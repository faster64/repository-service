using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Queries.GetListGpsInfo
{
    //[RequiredPermission(ClockingGpsPermission.Read)]
    public class GetGpsInfoByQrCodeQuery : QueryBase<GpsInfoDto>
    {
        public TenantModel Tenant { get; set; }
        public string QrKey { get; set; }
    }
}
