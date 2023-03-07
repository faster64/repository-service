using System;

namespace KiotVietTimeSheet.Application.Auth.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequiresAnyPermissionAttribute : Attribute
    {
        public string[] RequiredPermissions { get; }
        public RequiresAnyPermissionAttribute(params string[] permissions)
        {
            RequiredPermissions = permissions;
        }
    }
}
