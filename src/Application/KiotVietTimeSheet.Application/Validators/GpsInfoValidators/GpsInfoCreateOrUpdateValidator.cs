using System;
using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.GpsInfoValidators
{
    public class GpsInfoCreateOrUpdateValidator : AbstractValidator<GpsInfo>
    {
        public GpsInfoCreateOrUpdateValidator()
        {
            ValidateAddress();
            ValidateWardName();
            ValidateProvince();
            ValidateDistrict();
            ValidateRadiusLimit();
        }
        protected void ValidateAddress()
        {
            RuleFor(c => c.Address)
               .Must(c => !string.IsNullOrWhiteSpace(c))
               .WithMessage(string.Format(Message.not_empty, Label.gpsInfo_Address))
               .MaximumLength(100)
               .WithMessage(string.Format(Message.not_lessThan, Label.gpsInfo_Address, $"100"));
        }
        protected void ValidateWardName()
        {
            RuleFor(c => c.WardName)
               .Must(c => !string.IsNullOrWhiteSpace(c))
               .WithMessage(string.Format(Message.not_empty, Label.gpsInfo_WardName))
               .MaximumLength(100)
               .WithMessage(string.Format(Message.not_lessThan, Label.gpsInfo_WardName, $"100"));
        }
        protected void ValidateProvince()
        {
            RuleFor(c => c.Province)
               .Must(c => !string.IsNullOrWhiteSpace(c))
               .WithMessage(string.Format(Message.not_empty, Label.gpsInfo_Province))
               .MaximumLength(100)
               .WithMessage(string.Format(Message.not_lessThan, Label.gpsInfo_Province, $"100"));
        }
        protected void ValidateDistrict()
        {
            RuleFor(c => c.Province)
               .Must(c => !string.IsNullOrWhiteSpace(c))
               .WithMessage(string.Format(Message.not_empty, Label.gpsInfo_District))
               .MaximumLength(100)
               .WithMessage(string.Format(Message.not_lessThan, Label.gpsInfo_District, $"100"));
        }
        protected void ValidateRadiusLimit()
        {
            RuleFor(c => c.RadiusLimit)
               .InclusiveBetween(0, 1000)
               .WithMessage(string.Format(Message.number_between, Label.gpsInfo_RadiusLimit, $"0", $"1000")) ;

        }
    }
}
