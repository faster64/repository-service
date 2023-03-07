using System;
using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Validators
{
    public class ClockingHistoryValidator<T> : AbstractValidator<T> where T : ClockingHistory
    {
        public ClockingHistoryValidator() { }

        protected void ValidateCheckedInDate(DateTime clockingEndTime)
        {
            RuleFor(c => c.CheckedInDate)
                .LessThan(clockingEndTime).When(c => c.CheckedInDate != null)
                .WithMessage(Message.clockingHistory_checkedInDate);
        }

        protected void ValidateCheckedOutDate(DateTime clockingStartTime)
        {
            RuleFor(c => c.CheckedOutDate)
                .GreaterThanOrEqualTo(c => c.CheckedInDate).When(c => c.CheckedInDate != null && c.CheckedOutDate != null)
                .WithMessage(Message.clockingHistory_lessthanOutputTimeInputTime)
                .GreaterThan(clockingStartTime).When(c => c.CheckedOutDate != null)
                .WithMessage(Message.clockingHistory_greaterthanOutputTimeStartTime);
        }
    }
}
