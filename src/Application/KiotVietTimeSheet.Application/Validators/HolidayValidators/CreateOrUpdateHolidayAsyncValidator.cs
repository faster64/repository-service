using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Validators;
using Message = KiotVietTimeSheet.Resources.Message;

namespace KiotVietTimeSheet.Application.Validators.HolidayValidators
{
    public class CreateOrUpdateHolidayAsyncValidator : HolidayValidator<Holiday>
    {
        #region Properties
        private readonly IHolidayReadOnlyRepository _holidayReadOnlyRepository;
        #endregion

        #region Constructors
        public CreateOrUpdateHolidayAsyncValidator(
            IHolidayReadOnlyRepository holidayReadOnlyRepository
            )
        {
            _holidayReadOnlyRepository = holidayReadOnlyRepository;
            ValidateName();
            ValidateFrom();
            ValidateTo();

            CheckHolidayOverlapTimeWithHolidayInSystem();
        }
        #endregion

        protected void CheckHolidayOverlapTimeWithHolidayInSystem()
        {
            RuleFor(h => h)
                .MustAsync(async (holiday, token) => !await _holidayReadOnlyRepository.AnyBySpecificationAsync(
                        new FindHolidayByToGreaterThanOrEqualSpec(holiday.From)
                        .And(new FindHolidayByFromLessThanOrEqualSpec(holiday.To))
                        .Not(new FindHolidayByIdSpec(holiday.Id))))
                .WithMessage(Message.holiday_duplicatedHoliday);
        }
    }
}
