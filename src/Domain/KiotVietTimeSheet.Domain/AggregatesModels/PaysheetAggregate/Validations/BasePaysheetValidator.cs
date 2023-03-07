using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using Label = KiotVietTimeSheet.Resources.Label;
using Message = KiotVietTimeSheet.Resources.Message;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Validations
{
    public abstract class BasePaysheetValidator<T> : AbstractValidator<T> where T : Paysheet
    {
        protected void ValidateId()
        {
            RuleFor(c => c.Id).Must(id => id > 0).WithMessage(string.Format(Message.not_invalid, Label.id));
        }

        protected void ValidateCode()
        {
            RuleFor(c => c.Code)
                .MaximumLength(15)
                .WithMessage(string.Format(Message.not_lessThan, Label.paysheet_code, $"15"));
        }

        protected void ValidateWorkingDayNumber()
        {
            RuleFor(c => c)
                .Must(c => c.WorkingDayNumber >= 0)
                .WithMessage(Message.paysheet_minimumWorkingDayNumber)
                .Must(c => c.StartTime.AddDays(Constant.PaysheetPeriodOptionDays).Date >= c.EndTime.Date)
                .WithMessage(string.Format(Message.timeSheet_createPaysheetPeriodOptionDays, Constant.PaysheetPeriodOptionDays));
        }

        protected void ValidateName()
        {
            RuleFor(c => c.Name)
                .Must(c => !string.IsNullOrWhiteSpace(c))
                .WithMessage(Message.paysheet_EmptyName)
                .MaximumLength(50)
                .WithMessage(string.Format(Message.not_lessThan, Label.paysheet_name, $"50"));

        }

        protected void ValidateNote()
        {
            When(c => !string.IsNullOrWhiteSpace(c.Note), () =>
            {
                RuleFor(c => c.Note)
                    .MaximumLength(255)
                    .WithMessage(string.Format(Message.not_lessThan, Label.note, $"255"));
            });
        }
    }
}
