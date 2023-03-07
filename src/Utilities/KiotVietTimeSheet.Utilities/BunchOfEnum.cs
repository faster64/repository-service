using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KiotVietTimeSheet.Utilities
{
    public enum ContractTypes
    {
        [Description("Gói dùng thử")]
        Trial = 0,
        [Description("Gói cơ bản")]
        Basic = 1,
        [Description("Gói nâng cao")]
        Advance = 2
    }

    public enum PosParameterType
    {

        [Description("Dùng thử")]
        Trial = 0,
        [Description("Dùng thật")]
        Paid = 1
    }

    public enum ImportExportStatus
    {
        Processing = 1,
        Completed = 2,
        Error = 3,
        Downloaded = 4,
    }

    public enum ImportStatus
    {
        Success = 1,
        Error = 2,
    }

    public enum AutoGenerateClockingStatus
    {
        NoAuto = 0,
        Auto = 1
    }

    public enum GpsInfoStatus
    {
        Inactive = 0,
        Active = 1
    }

    public enum ConfirmClockingType
    {
        [Display(Name = "Chưa xác định")]
        Unknown = 0,

        [Display(Name = "Chấm công trên thiết bị chưa được đăng ký")]
        NewDevice = 1,

        [Display(Name = "Chấm công ở vị trí khác với địa điểm cần chấm công")]
        WrongGps = 2,

        [Display(Name = "Chấm công trên thiết bị chưa được đăng ký và ở vị trí khác với địa điểm cần chấm công")]
        NewDeviceAndWrongGps = 3,

        [Display(Name = "Không chia sẻ GPS")]
        NotShareLocation = 4,

        [Display(Name = "Chấm công trên thiết bị chưa được đăng ký và không chia sẻ GPS")]
        NewDeviceAndNotShareLocation = 5,
    }

    public enum ConfirmClockingStatus
    {
        Waiting = 0,
        Confirm = 1,
        Reject = 2
    }
    public enum PayRateTemplateStatus
    {
        New = 0,
        Deleted = 1
    }
}
