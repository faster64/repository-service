using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Utilities;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetAndCheckTwoFaPin
{
    public class CheckTwoFaPinQueryHandler : QueryHandlerBase,
        IRequestHandler<CheckTwoFaPinQuery, bool>
    {
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IEventDispatcher _eventDispatcher;

        public CheckTwoFaPinQueryHandler(
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IAuthService authService,
            IEventDispatcher eventDispatcher
            ) : base(authService)
        {
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<bool> Handle(CheckTwoFaPinQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeWriteOnlyRepository.FindByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(typeof(Employee).Name, string.Format(Message.not_existsInSystem, Label.employee)));
                return false;
            }

            if (string.IsNullOrEmpty(employee.AccountSecretKey)) return false;

            var result = Globals.ValidateTwoFactorPin(EncryptHelpers.RijndaelDecrypt(employee.AccountSecretKey), Globals.EmployeeConfirmPinInSeconds, request.Pin);

            return result;
        }
    }
}
