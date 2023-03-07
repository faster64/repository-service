using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Validators.EmployeeValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Queries.CheckEmployeeAssignUserId
{
    public class CheckEmployeeAssignUserIdQueryHandler : QueryHandlerBase,
        IRequestHandler<CheckEmployeeAssignUserIdQuery, bool>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;

        public CheckEmployeeAssignUserIdQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IEventDispatcher eventDispatcher,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
            ) : base(authService)
        {
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<bool> Handle(CheckEmployeeAssignUserIdQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeReadOnlyRepository.FindByIdAsync(request.EmployeeId);

            if (employee == null)
            {
                NotifyEmployeeInDbIsNotExists();
                return false;
            }

            var newEmployee = employee.CreateCopy();
            newEmployee.UpdateUserId(request.UserId);

            var validator = await (new AssignUserIdToEmployeeValidator(request.IsCreateUser, employee.UserId).ValidateAsync(newEmployee));
            if (!validator.IsValid)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(nameof(Employee), validator.Errors.First().ErrorMessage));
                return false;
            }

            return true;
        }

        #region PRIVATE METHODS
        private void NotifyEmployeeInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Employee).Name, string.Format(Message.not_exists, Label.employee)));
        }
        #endregion
    }
}
