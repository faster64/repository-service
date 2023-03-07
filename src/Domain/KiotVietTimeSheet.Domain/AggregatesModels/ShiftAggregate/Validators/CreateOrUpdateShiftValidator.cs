using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Validators
{
    public class CreateOrUpdateShiftValidator : ShiftValidator<Shift>
    {
        public CreateOrUpdateShiftValidator()
        {
            ValidateName();
            ValidateTo();
            ValidateFrom();
        }
    }
}
