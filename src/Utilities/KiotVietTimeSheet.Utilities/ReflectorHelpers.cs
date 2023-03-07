using System;

namespace KiotVietTimeSheet.Utilities
{
    public static class ReflectorHelpers
    {
        public static bool HasInterface(this Type type, Type checkType)
        {
            return checkType != null && type.GetInterface(checkType.Name) != null;
        }
    }
}
