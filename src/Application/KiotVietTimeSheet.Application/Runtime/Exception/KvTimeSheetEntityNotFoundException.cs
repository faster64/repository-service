using KiotVietTimeSheet.Resources;
using System;
using System.Runtime.Serialization;

namespace KiotVietTimeSheet.Application.Runtime.Exception
{
    [Serializable]
    public class KvTimeSheetEntityNotFoundException : KvTimeSheetException
    {
        public KvTimeSheetEntityNotFoundException(string msg)
            : base(string.IsNullOrWhiteSpace(msg) ? string.Format(Resources.Message.not_exists, Label.record) : msg)
        { }
        protected KvTimeSheetEntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // ...
        }
    }
}
