using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetClockingsForClockingGps
{
    public class GetClockingsForClockingGpsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetClockingsForClockingGpsQuery, ClockingGpsDto>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IKiotVietInternalService _kiotVietInternalService;
        private readonly IAuthService _authService;
        private readonly IConfirmClockingReadOnlyRepository _confirmClockingReadOnlyRepository;

        public GetClockingsForClockingGpsQueryHandler(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IKiotVietInternalService kiotVietInternalService,
            IAuthService authService,
            IConfirmClockingReadOnlyRepository confirmClockingReadOnlyRepository
        ) : base(authService)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _kiotVietInternalService = kiotVietInternalService;
            _authService = authService;
            _confirmClockingReadOnlyRepository = confirmClockingReadOnlyRepository;
        }

        public async Task<ClockingGpsDto> Handle(GetClockingsForClockingGpsQuery request, CancellationToken cancellationToken)
        {
            var clockings = await _clockingReadOnlyRepository.GetClockingsForClockingGps(request.BranchId, request.EmployeeId);
            var branch = await _kiotVietInternalService.GetBranchByIdAsync(request.BranchId, _authService.Context.TenantId);
            var now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, now.Day);
            DateTime end = start.AddDays(1).AddTicks(-1);
            var confirmClocking = await _confirmClockingReadOnlyRepository.GetConfirmClockingForClockingGps(_authService.Context.TenantId, request.BranchId, request.EmployeeId, start, end);

            return new ClockingGpsDto { ClockingsDto = clockings, BranchName = branch.Name, ConfirmClockingDto = confirmClocking };
        }
    }
}
