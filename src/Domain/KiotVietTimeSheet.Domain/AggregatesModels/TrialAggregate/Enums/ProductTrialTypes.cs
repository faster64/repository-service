using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TrialAggregate.Enums
{
    public enum ProductTrialTypes
    {
        [Description("Retail")]
        Retail = 1,
        [Description("FnB")]
        FnB = 2,
        [Description("Booking")]
        Booking = 3
    }
}
