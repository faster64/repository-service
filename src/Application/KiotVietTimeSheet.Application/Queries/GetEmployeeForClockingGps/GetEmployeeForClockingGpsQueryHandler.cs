using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Utilities;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeForClockingGps
{
    public class GetEmployeeForClockingGpsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeForClockingGpsQuery, object>
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IEventDispatcher _eventDispatcher;
        public GetEmployeeForClockingGpsQueryHandler(IEventDispatcher eventDispatcher,
            IAuthService authService,
            IMapper mapper,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
        ) : base(authService)
        {
            _authService = authService;
            _mapper = mapper;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<object> Handle(GetEmployeeForClockingGpsQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeReadOnlyRepository.GetEmployeeForClockingGps(_authService.Context.TenantId, request.Keyword, request.IsPhone);

            if (employee == null || employee.Id <= 0)
            {
                await _eventDispatcher.FireEvent(new DomainNotification("Clocking GPS Errors", new ErrorResult()
                {
                    Code = "wrong_phone_number_or_code",
                    Message = request.IsPhone ? "Số điện thoại chưa tồn tại trong hệ thống. Vui lòng liên hệ với quản lý để đăng ký." : "Mã nhân viên chưa chính xác. Vui lòng kiểm tra lại"
                }));

                return null;
            }

            if (string.IsNullOrWhiteSpace(employee.IdentityKeyClocking))
            {
                await _eventDispatcher.FireEvent(new DomainNotification("Clocking GPS Errors", new ErrorResult()
                {
                    Code = "new_device",
                    Message = "Nhân viên chưa đăng nhập với thiết bị nào."
                }));

                return new { Employee = _mapper.Map<EmployeeDto>(employee) };
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(employee.TenantId);
            sb.Append(employee.BranchId);
            sb.Append(employee.Id);
            sb.Append(employee.Code);
            sb.Append(request.Os);
            sb.Append(request.OsVersion);
            sb.Append(request.Vendor);
            sb.Append(request.Model);
            sb.Append(request.Type);

            var identityKeyClocking = EncryptHelpers.GetHashString(sb.ToString());
            if (!identityKeyClocking.Equals(employee.IdentityKeyClocking))
            {
                await _eventDispatcher.FireEvent(new DomainNotification("Clocking GPS Errors", new ErrorResult()
                {
                    Code = "wrong_device",
                    Message = "Nhân viên đã đăng nhập với thiết bị mới."
                }));
                return new { Employee = _mapper.Map<EmployeeDto>(employee), IdentityKeyClocking = identityKeyClocking };
            }
            return new { Employee = _mapper.Map<EmployeeDto>(employee) };
        }

    }
}
