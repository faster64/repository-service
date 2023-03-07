using System;

namespace KiotVietTimeSheet.Application.Auth.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequiredPermissionAttribute : Attribute
    {
        public string[] RequiredPermissions { get; set; }
        public RequiredPermissionAttribute(params string[] permissions)
        {
            RequiredPermissions = permissions;
        }
    }
}
