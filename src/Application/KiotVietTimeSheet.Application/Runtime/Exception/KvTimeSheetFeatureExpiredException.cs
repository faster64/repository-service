using KiotVietTimeSheet.SharedKernel.Models;
using System;
using System.Runtime.Serialization;

namespace KiotVietTimeSheet.Application.Runtime.Exception
{
    [Serializable]
    public class KvTimeSheetFeatureExpiredException : KvTimeSheetException
    {
        public ErrorResult ErrorResult { get; set; }
        public KvTimeSheetFeatureExpiredException(string msg)
            : base(msg)
        {

        }
        public KvTimeSheetFeatureExpiredException(ErrorResult errorResult)
            : base(errorResult.Message)
        {
            ErrorResult = errorResult;
        }
        protected KvTimeSheetFeatureExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // ...
        }
    }
}
