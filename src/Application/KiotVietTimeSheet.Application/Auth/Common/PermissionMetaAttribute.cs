using System;

namespace KiotVietTimeSheet.Application.Auth.Common
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PermissionMetaAttribute : Attribute
    {
        public string Name { get; set; }
        public string[] Parents { get; set; }
        public string[] Children { get; set; }
        public bool Disable { get; set; }
        public PermissionMetaAttribute()
        {

        }
    }
}
