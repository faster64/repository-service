﻿using System;
using System.Runtime.Serialization;

namespace KiotVietTimeSheet.BackgroundTasks.Exceptions
{
    [Serializable]
    public sealed class KvTimeSheetPayslipNullException : Exception
    {
        public KvTimeSheetPayslipNullException(string msg) : base(msg) { }
        private KvTimeSheetPayslipNullException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
