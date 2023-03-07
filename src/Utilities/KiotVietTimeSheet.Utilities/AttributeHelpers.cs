using System;
using System.Linq;
using System.Reflection;

namespace KiotVietTimeSheet.Utilities
{
    public static class AttributeHelpers
    {
        public static TOut GetConstFieldAttributeValue<T, TOut, TAttribute>(string fieldName, Func<TAttribute, TOut> valueSelector) where TAttribute : Attribute
        {
            var fieldInfo = typeof(T).GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
            if (fieldInfo == null)
            {
                return default(TOut);
            }
            var attr = fieldInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            return attr != null ? valueSelector(attr) : default(TOut);
        }
    }
}
