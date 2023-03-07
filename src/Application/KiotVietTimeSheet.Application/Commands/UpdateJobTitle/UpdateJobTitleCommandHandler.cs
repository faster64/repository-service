using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.EmployeeValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdateJobTitle
{
    public class UpdateJobTitleCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateJobTitleCommand, Unit>
    {
        private readonly IJobTitleWriteOnlyRepository _jobTitleWriteOnlyRepository;
        private readonly JobTitleCreateOrUpdateValidator _jobTitleCreateOrUpdateValidator;
        public UpdateJobTitleCommandHandler(
            IEventDispatcher eventDispatcher,
            JobTitleCreateOrUpdateValidator jobTitleCreateOrUpdateValidator,
            IJobTitleWriteOnlyRepository jobTitleWriteOnlyRepository
        ) : base(eventDispatcher)
        {
            _jobTitleWriteOnlyRepository = jobTitleWriteOnlyRepository;
            _jobTitleCreateOrUpdateValidator = jobTitleCreateOrUpdateValidator;
        }

        public async Task<Unit> Handle(UpdateJobTitleCommand request, CancellationToken cancellationToken)
        {
            var jobTitleDto = request.JobTitle;
            var existingJobTitle = await _jobTitleWriteOnlyRepository.FindByIdAsync(jobTitleDto.Id);

            if (existingJobTitle == null)
            {
                NotifyValidationErrors(typeof(JobTitle), new List<string> { string.Format(Message.not_exists, Label.jobTitle) });
                return Unit.Value;
            }

            existingJobTitle.Update(jobTitleDto.Name, jobTitleDto.Description, jobTitleDto.IsActive);

            var validator = await _jobTitleCreateOrUpdateValidator.ValidateAsync(existingJobTitle, cancellationToken);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(JobTitle), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return Unit.Value;
            }

            _jobTitleWriteOnlyRepository.Update(existingJobTitle);

            await _jobTitleWriteOnlyRepository.UnitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
