using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents;
using KiotVietTimeSheet.Application.Queries.GetAndCheckTwoFaPin;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Utilities;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdateDeviceForEmployeeClockingGps
{
    public class UpdateDeviceForEmployeeClockingGpsCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateDeviceForEmployeeClockingGpsCommand, EmployeeDto>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IMediator _mediator;

        public UpdateDeviceForEmployeeClockingGpsCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IMediator mediator
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _mediator = mediator;
        }

        public async Task<EmployeeDto> Handle(UpdateDeviceForEmployeeClockingGpsCommand request, CancellationToken cancellationToken)
        {
            var isValid = await _mediator.Send(new CheckTwoFaPinQuery(request.EmployeeId, request.VerifyCode));
            if (!isValid)
            {
                // Cần update check mã xác nhận
                await _eventDispatcher.FireEvent(new DomainNotification("Clocking GPS", "Mã xác nhận không hợp lệ hoặc hết hạn."));
                return null;
            }

            var employee = await _employeeReadOnlyRepository.GetByIdWithoutLimit(request.EmployeeId);

            if (employee == null)
            {
                await _eventDispatcher.FireEvent(new DomainNotification("Clocking GPS", Message.timeSheet_employeesNotExist));
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(employee.TenantId);
            sb.Append(employee.BranchId);
            sb.Append(employee.Id);
            sb.Append(employee.Code);
            sb.Append(request.Os);
            sb.Append(request.OsVersion);
            sb.Append(request.Type);

            var identityKeyClocking = EncryptHelpers.GetHashString(sb.ToString());

            if (!string.IsNullOrWhiteSpace(employee.IdentityKeyClocking) && employee.IdentityKeyClocking != identityKeyClocking)
            {
                await _eventDispatcher.FireEvent(new DomainNotification("Clocking GPS", "Đã kết nối với 1 thiết bị trước đó."));
                return null;
            }

            var employeeByIdentityClocking = await _employeeReadOnlyRepository.GetByIdentityKeyClockingWithoutLimit(identityKeyClocking);
            if (employeeByIdentityClocking != null && employeeByIdentityClocking.Id != request.EmployeeId)
            {
                await _eventDispatcher.FireEvent(new DomainNotification("Clocking GPS", "Thiết bị di động đã được đăng ký chấm công với một nhân viên trong hệ thống. Vui lòng đăng ký thiết bị di động khác để chấm công."));
                return null;
            }

            employee.UpdateIdentityKeyClocking(identityKeyClocking);
            _employeeWriteOnlyRepository.Update(employee);
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdateEmployeeDeviceIntegrationEvent(employee.BranchId, (employee.SecretKeyTakenUserId ?? employee.UserId) ?? 0, employee.Code, identityKeyClocking));
            await _employeeWriteOnlyRepository.UnitOfWork.CommitAsync();

            return _mapper.Map<EmployeeDto>(employee);
        }
    }
}