using System;
using System.Runtime.Serialization;

namespace KiotVietTimeSheet.Application.Runtime.Exception
{
    [Serializable]
    public class KvTimeSheetModuleUnActiveException : KvTimeSheetException
    {
        public KvTimeSheetModuleUnActiveException()
            : base("Tính năng quản lý nhân viên của gian hàng chưa được kích hoạt")
        { }

        protected KvTimeSheetModuleUnActiveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // ...
        }
    }
}
