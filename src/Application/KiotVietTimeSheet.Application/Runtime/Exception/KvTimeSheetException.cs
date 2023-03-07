
using System;
using System.Runtime.Serialization;

namespace KiotVietTimeSheet.Application.Runtime.Exception
{
    [Serializable]
    public class KvTimeSheetException : System.Exception
    {
        public KvTimeSheetException(string msg)
            : base(msg) { }
        protected KvTimeSheetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // ...
        }
    }
}
