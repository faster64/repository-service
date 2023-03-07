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
    public class GetTwoFaPinQueryHandler : QueryHandlerBase,
        IRequestHandler<GetTwoFaPinQuery, string>
    {
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IEventDispatcher _eventDispatcher;

        public GetTwoFaPinQueryHandler(
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IAuthService authService,
            IEventDispatcher eventDispatcher
            ) : base(authService)
        {
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<string> Handle(GetTwoFaPinQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeWriteOnlyRepository.FindByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(typeof(Employee).Name, string.Format(Message.not_existsInSystem, Label.employee)));
                return null;
            }

            if (string.IsNullOrEmpty(employee.AccountSecretKey) || employee.SecretKeyTakenUserId == null) employee = await _employeeWriteOnlyRepository.UpdateNewAccountSecretKey(request.EmployeeId, request.UserId);

            var result = Globals.GetTwoFaPin(EncryptHelpers.RijndaelDecrypt(employee.AccountSecretKey), Globals.EmployeeConfirmPinInSeconds);

            return result;
        }
    }
}
