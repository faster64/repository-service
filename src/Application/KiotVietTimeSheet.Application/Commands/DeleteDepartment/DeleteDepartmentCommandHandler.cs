using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteDepartmentCommand, Unit>
    {
        private readonly IDepartmentWriteOnlyRepository _departmentWriteOnlyRepository;
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;


        public DeleteDepartmentCommandHandler(
            IEventDispatcher eventDispatcher,
            IDepartmentWriteOnlyRepository departmentWriteOnlyRepository,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository
        ) : base(eventDispatcher)
        {
            _departmentWriteOnlyRepository = departmentWriteOnlyRepository;
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
        }

        public async Task<Unit> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            long id = request.Id;
            var existingDepartment = await _departmentWriteOnlyRepository.FindByIdAsync(id);

            if (existingDepartment == null)
            {
                NotifyValidationErrors(typeof(Department), new List<string> { string.Format(Message.not_exists, Label.department) });
                return Unit.Value;
            }

            existingDepartment.Delete();
            var employeesInDepartment = await _employeeWriteOnlyRepository.GetBySpecificationAsync(new GetEmployeeByDepartmentIdSpec(existingDepartment.Id));
            if (employeesInDepartment.Any())
            {
                employeesInDepartment.Each(e => e.LeftDeparment());
                _employeeWriteOnlyRepository.BatchUpdate(employeesInDepartment);
            }

            _departmentWriteOnlyRepository.Delete(existingDepartment);

            await _departmentWriteOnlyRepository.UnitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
