using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Validators
{
    public class CreateOrUpdateHolidayValidator : HolidayValidator<Holiday>
    {
        public CreateOrUpdateHolidayValidator()
        {
            ValidateName();
            ValidateFrom();
            ValidateTo();
        }
    }
}
