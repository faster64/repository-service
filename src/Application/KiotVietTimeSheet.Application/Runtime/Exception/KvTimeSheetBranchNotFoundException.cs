using System;
using System.Runtime.Serialization;

namespace KiotVietTimeSheet.Application.Runtime.Exception
{
    [Serializable]
    public class KvTimeSheetBranchNotFoundException : KvTimeSheetException
    {
        public KvTimeSheetBranchNotFoundException()
            : base("Chi nhánh không tồn tại")
        { }
        protected KvTimeSheetBranchNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // ...
        }
    }
}
