using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetSetting
{
    public sealed class GetSettingQuery : QueryBase<SettingObjectDto>
    {
        public int TenantId { get; set; }

        public GetSettingQuery(int tenantId)
        {
            TenantId = tenantId;
        }
    }
}
