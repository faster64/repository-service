using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Validations;
using Message = KiotVietTimeSheet.Resources.Message;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using Label = KiotVietTimeSheet.Resources.Label;

namespace KiotVietTimeSheet.Application.Validators.EmployeeValidators
{
    public class JobTitleCreateOrUpdateValidator : BaseJobTitleValidator<JobTitle>
    {
        #region Properties
        private readonly IJobTitleReadOnlyRepository _jobTitleReadOnlyRepository;
        #endregion

        #region Constructor
        public JobTitleCreateOrUpdateValidator(IJobTitleReadOnlyRepository jobTitleReadOnlyRepository)
        {
            _jobTitleReadOnlyRepository = jobTitleReadOnlyRepository;

            ValidateName();
            ValidateDescription();
            ValidateJobTitleIsExistsByNameAsync();
        }
        #endregion

        #region Protected methods
        protected void ValidateJobTitleIsExistsByNameAsync()
        {
            RuleFor(e => e)
                .MustAsync(async (jobTitle, token) =>
                {
                    var spec = (new FindJobTitleByNameSpec(jobTitle.Name)).Not(new FindJobTitleByIdSpec(jobTitle.Id));
                    var existingJobTitle = await _jobTitleReadOnlyRepository.FindBySpecificationAsync(spec);

                    if (existingJobTitle != null)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(string.Format(Message.is_existsInSystem, Label.jobTitle_name));
        }
        #endregion
    }
}
