using System;
using System.Runtime.Serialization;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Exceptions
{
    [Serializable]
    public class TimeKeepingLogEmployeeNotFoundException : Exception
    {
        public TimeKeepingLogEmployeeNotFoundException() : base("Không tìm thấy ca làm việc phù hợp với dữ liệu chấm công này")
        {

        }
        protected TimeKeepingLogEmployeeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // ...
        }
    }
}
