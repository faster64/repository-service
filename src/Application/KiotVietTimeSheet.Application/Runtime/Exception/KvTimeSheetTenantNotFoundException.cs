using KiotVietTimeSheet.Resources;
using System;
using System.Runtime.Serialization;

namespace KiotVietTimeSheet.Application.Runtime.Exception
{
    [Serializable]
    public class KvTimeSheetTenantNotFoundException : KvTimeSheetException
    {
        public KvTimeSheetTenantNotFoundException()
            : base(string.Format(Resources.Message.not_exists, Label.store))
        { }
        protected KvTimeSheetTenantNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // ...
        }
    }
}
