using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.DeleteJobTitle
{
    public class DeleteJobTitleCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteJobTitleCommand, Unit>
    {
        private readonly IJobTitleWriteOnlyRepository _jobTitleWriteOnlyRepository;
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        public DeleteJobTitleCommandHandler(
            IEventDispatcher eventDispatcher,
            IJobTitleWriteOnlyRepository jobTitleWriteOnlyRepository,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository

        ) : base(eventDispatcher)
        {
            _jobTitleWriteOnlyRepository = jobTitleWriteOnlyRepository;
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
        }

        public async Task<Unit> Handle(DeleteJobTitleCommand request, CancellationToken cancellationToken)
        {
            var existingJobTitle = await _jobTitleWriteOnlyRepository.FindByIdAsync(request.Id);

            if (existingJobTitle == null)
            {
                NotifyValidationErrors(typeof(JobTitle), new List<string> { string.Format(Message.not_exists, Label.jobTitle) });
                return Unit.Value;
            }

            existingJobTitle.Delete();

            var employeesInJobTitle = await _employeeWriteOnlyRepository.GetBySpecificationAsync(new GetEmployeeByJobTitleIdSpec(existingJobTitle.Id));
            if (employeesInJobTitle.Any())
            {
                employeesInJobTitle.Each(e => e.LeftJobTitle());

                _employeeWriteOnlyRepository.BatchUpdate(employeesInJobTitle);
            }

            _jobTitleWriteOnlyRepository.Delete(existingJobTitle);

            await _jobTitleWriteOnlyRepository.UnitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
