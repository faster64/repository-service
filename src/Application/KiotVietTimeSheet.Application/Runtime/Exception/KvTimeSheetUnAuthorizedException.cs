using System;
using System.Runtime.Serialization;

namespace KiotVietTimeSheet.Application.Runtime.Exception
{
    [Serializable]
    public class KvTimeSheetUnAuthorizedException : KvTimeSheetException
    {
        public KvTimeSheetUnAuthorizedException(string msg)
            : base(msg)
        {

        }
        protected KvTimeSheetUnAuthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // ...
        }
    }
}
