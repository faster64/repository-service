namespace KiotVietTimeSheet.Domain.Common
{
    public static class Constant
    {
        public static readonly string RegexPatternForSpecChar = @"^[a-zA-Z0-9 áàạảãâấầậẩẫăắằặẳẵÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴéèẹẻẽêếềệểễÉÈẸẺẼÊẾỀỆỂỄóòọỏõôốồộổỗơớờợởỡÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠúùụủũưứừựửữÚÙỤỦŨƯỨỪỰỬỮíìịỉĩÍÌỊỈĨđĐýỳỵỷỹÝỲỴỶỸ]*$";
        public static readonly string RegexPatternForOnlyNumber = @"^\d*$";
        public static readonly string RegexPatternForEmail = @"/\b[\w\.-]+@[\w\.-]+\.\w{2,4}\b/gi";
        public static readonly int QuotaOfBasicContract = 3;
        public static readonly double NumberOfHourOverLapBetween2Ranges = 2;
        public static readonly int MaximumTimeSheetCanUpdate = 100;
        public static readonly int MaximumClockingCanUpdate = 3000;
        public static readonly int PaysheetMaximumMonths = 3;
        public static readonly string CodeDelSuffix = "{DEL}";
        public const double Tolerance = .000001;
        public static readonly int MaximumShiftIsAutoTimekeeping = 3;
        public static readonly int MinimumShiftIsAutoTimekeeping = 2;
        public static readonly int MaximumRangeShiftIsAutoTimekeepingHours = 24;
        public static readonly int MaximumRangeShiftIsAutoTimekeepingMinutes = 59;
        public const int MaximumCheckInCheckOutHours = 10;
        public const int MillisecondsDelay = 2000;
        public const string TaskCanceledExceptionMessage = "Lỗi kết nối với kiotviet vui lòng thử lại";
        public const int PaysheetPeriodOptionDays = 90;
        public const int SyncEmployeeDefaultPageSize = 50;
    }
}
