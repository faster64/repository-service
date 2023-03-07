using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Auth
{
    public class ClockingGpsPermission
    {
        protected ClockingGpsPermission()
        {
        }

        [PermissionMeta(Name = "Full")]
        public const string Full = "full";

        [PermissionMeta(Name = "Read", Parents = new[] { Full })]
        public const string Read = "read";
    }
}
