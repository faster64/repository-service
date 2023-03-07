using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeForClockingGps
{
    [RequiredPermission(ClockingGpsPermission.Full)]
    public class GetEmployeeForClockingGpsQuery : QueryBase<object>
    {
        public string IdentityKeyClocking { get; set; }
        public string Keyword { get; set; }
        public bool IsPhone { get; set; }
        public string Os { get; set; }
        public string OsVersion { get; set; }
        public string Vendor { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public GetEmployeeForClockingGpsQuery(
            string identityKeyClocking,
            string os,
            string osVersion,
            string vendor,
            string model,
            string type,
            string keyword,
            bool isPhone)
        {
            IdentityKeyClocking = identityKeyClocking;
            Os = os;
            OsVersion = osVersion;
            Vendor = vendor;
            Model = model;
            Type = type;
            Keyword = keyword;
            IsPhone = isPhone;
        }
    }
}
