using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums
{
    public enum AbsenceTypes
    {
        [Description("Có phép")]
        AuthorisedAbsence = 1,
        [Description("Không phép")]
        UnauthorisedAbsence = 2
    }
}
