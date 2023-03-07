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
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeByIdentityKeyForClockingGps
{
    public class GetEmployeeByIdentityKeyForClockingGpsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeByIdentityKeyForClockingGpsQuery, EmployeeDto>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IEventDispatcher _eventDispatcher;
        public GetEmployeeByIdentityKeyForClockingGpsQueryHandler(IEventDispatcher eventDispatcher,
            IAuthService authService,
            IMapper mapper,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
        ) : base(authService)
        {
            _mapper = mapper;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<EmployeeDto> Handle(GetEmployeeByIdentityKeyForClockingGpsQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeReadOnlyRepository.GetByIdentityKeyClockingWithoutLimit(request.IdentityKeyClocking);

            if (employee == null || employee.Id <= 0)
            {
                await _eventDispatcher.FireEvent(new DomainNotification("Clocking GPS Errors", new ErrorResult()
                {
                    Code = "employee_not_found",
                    Message = "Nhân viên không tồn tại."
                }));

                return null;
            }

            return _mapper.Map<EmployeeDto>(employee);
        }

    }
}
