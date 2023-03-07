using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UnAssignUserEmployee
{
    public class UnAssignUserEmployeeCommandHandler : BaseCommandHandler,
        IRequestHandler<UnAssignUserEmployeeCommand, Unit>
    {
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        public UnAssignUserEmployeeCommandHandler(
            IEventDispatcher eventDispatcher,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository
            )
            : base(eventDispatcher)
        {
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
        }

        public async Task<Unit> Handle(UnAssignUserEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeWriteOnlyRepository.FindBySpecificationAsync(new FindEmployeeByUserIdSpec(request.UserId));
            if (employee != null)
            {
                employee.UnAssignUser();
                _employeeWriteOnlyRepository.Update(employee);
                await _employeeWriteOnlyRepository.UnitOfWork.CommitAsync();
            }
            return Unit.Value;
        }
    }
}
