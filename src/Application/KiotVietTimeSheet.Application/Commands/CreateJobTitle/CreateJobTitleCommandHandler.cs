using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.EmployeeValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateJobTitle
{
    public class CreateJobTitleCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateJobTitleCommand, JobTitleDto>
    {
        private readonly IMapper _mapper;
        private readonly IJobTitleWriteOnlyRepository _jobTitleWriteOnlyRepository;
        private readonly JobTitleCreateOrUpdateValidator _jobTitleCreateOrUpdateValidator;
        public CreateJobTitleCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            JobTitleCreateOrUpdateValidator jobTitleCreateOrUpdateValidator,
            IJobTitleWriteOnlyRepository jobTitleWriteOnlyRepository
           )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _jobTitleWriteOnlyRepository = jobTitleWriteOnlyRepository;
            _jobTitleCreateOrUpdateValidator = jobTitleCreateOrUpdateValidator;
        }

        public async Task<JobTitleDto> Handle(CreateJobTitleCommand request, CancellationToken cancellationToken)
        {
            var jobTitleDto = request.JobTitle;
            var jobTitle = new JobTitle(jobTitleDto.Name, jobTitleDto.Description, jobTitleDto.IsActive);

            var validator = await _jobTitleCreateOrUpdateValidator.ValidateAsync(jobTitle, cancellationToken);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(JobTitle), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return jobTitleDto;
            }

            _jobTitleWriteOnlyRepository.Add(jobTitle);
            await _jobTitleWriteOnlyRepository.UnitOfWork.CommitAsync();

            return _mapper.Map<JobTitleDto>(jobTitle);
        }
    }
}
