using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.Resources;
using System.Linq;

namespace KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Validations
{
    public class BranchSettingValidator<T> : AbstractValidator<T> where T : BranchSetting
    {
        protected void ValidateWorkingDays()
        {
            RuleFor(s => s.WorkingDays)
               .Must(s =>
               {
                   if (string.IsNullOrEmpty(s))
                   {
                       return true;
                   }
                   var str = string.IsNullOrWhiteSpace(s) ? string.Empty : s;
                   var workingDaysInArray = str.Split(',').Select(d =>
                   {
                       if (byte.TryParse(d, out byte result))
                       {
                           return result;
                       }

                       return -1;
                   }).ToArray();
                   return workingDaysInArray.All(d => BranchSetting.DefaultWorkingDays.Contains((byte)d));
               })
               .WithMessage(string.Format(Message.not_invalid, Label.workingDay));
        }
    }
}
