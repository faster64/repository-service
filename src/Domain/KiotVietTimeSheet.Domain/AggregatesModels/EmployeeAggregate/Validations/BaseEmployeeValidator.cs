using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Validations
{
    public abstract class BaseEmployeeValidator<T> : AbstractValidator<T> where T : Employee
    {
        protected void ValidateId()
        {
            RuleFor(c => c.Id).Must(id => id > 0).WithMessage(string.Format(Message.not_invalid, Label.id));
        }

        protected void ValidateCode()
        {
            RuleFor(c => c.Code)
                .MaximumLength(10)
                .WithMessage(string.Format(Message.not_lessThan, Label.employee_code, $"10"));

            When(c => c.Id > 0, () =>
            {
                RuleFor(c => c.Code)
                    .Must(c => !string.IsNullOrWhiteSpace(c))
                    .WithMessage(string.Format(Message.not_empty, Label.employee_code));
            });
        }

        protected void ValidateName()
        {
            RuleFor(c => c.Name)
                .Must(c => !string.IsNullOrWhiteSpace(c))
                .WithMessage(string.Format(Message.not_empty, Label.employee_name))
                .MaximumLength(50)
                .WithMessage(string.Format(Message.not_lessThan, Label.employee_name, $"50"));
        }

        protected void ValidateIdentityNumber()
        {
            When(c => !string.IsNullOrWhiteSpace(c.IdentityNumber), () =>
            {
                RuleFor(c => c.IdentityNumber)
                    .MaximumLength(15)
                    .WithMessage(string.Format(Message.not_lessThan, Label.employee_CMDN, $"15"));
            });
        }

        protected void ValidateNote()
        {
            When(c => !string.IsNullOrWhiteSpace(c.Note), () =>
            {
                RuleFor(c => c.Note)
                    .MaximumLength(500)
                    .WithMessage(string.Format(Message.not_lessThan, Label.note, $"500"));
            });
        }

        protected void ValidatePhoneNumber()
        {
            When(c => !string.IsNullOrWhiteSpace(c.MobilePhone), () =>
            {
                RuleFor(c => c.MobilePhone)
                    .MaximumLength(15)
                    .WithMessage(string.Format(Message.not_lessThan, Label.mobilePhone, $"15"))
                    .Matches(@"^[0-9 .,+-]+$")
                    .WithMessage(string.Format(Message.not_invalid, Label.mobilePhone));
            });
        }

        protected void ValidateEmail()
        {
            When(c => !string.IsNullOrWhiteSpace(c.Email), () =>
            {
                RuleFor(c => c.Email)
                    .MaximumLength(50)
                    .WithMessage(string.Format(Message.not_lessThan, Label.employee_email, $"50"))
                    .EmailAddress()
                    .WithMessage(string.Format(Message.not_fomatInvalid, Label.employee_email));
            });
        }

        protected void ValidateFacebook()
        {
            When(c => !string.IsNullOrWhiteSpace(c.Facebook), () =>
            {
                RuleFor(c => c.Facebook)
                    .MaximumLength(50)
                    .WithMessage(string.Format(Message.not_lessThan, Label.facebook, $"50"));
            });
        }

        protected void ValidateAddress()
        {
            When(c => !string.IsNullOrWhiteSpace(c.Address), () =>
            {
                RuleFor(c => c.Address)
                    .MaximumLength(255)
                    .WithMessage(string.Format(Message.not_lessThan, Label.employee_address, $"255"));
            });
        }

        protected void ValidateProfilePictures()
        {
            When(c => c.ProfilePictures != null, () =>
            {
                RuleFor(c => c.ProfilePictures)
                    .Must(c => c.Count <= 5)
                    .WithMessage(string.Format(Message.allow_maximumPicture, Label.employee_picture, $"5"));
            });
        }
    }
}
