using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetClockingsMultipleBranch
{
    public class GetClockingsMultipleBranchForCalendarQueryHandler : QueryHandlerBase,
        IRequestHandler<GetClockingsMultipleBranchForCalendarQuery, PagingDataSource<ClockingDto>>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;

        public GetClockingsMultipleBranchForCalendarQueryHandler(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IAuthService authService
        ) : base(authService)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
        }

        public async Task<PagingDataSource<ClockingDto>> Handle(GetClockingsMultipleBranchForCalendarQuery request, CancellationToken cancellationToken)
        {
            var clockings = await _clockingReadOnlyRepository.GetClockingMultipleBranchForCalendars(request.BranchIds, request.ClockingHistoryStates,
                request.DepartmentIds, request.ShiftIds, request.EmployeeIds, request.StartTime, request.EndTime, request.ClockingStatusExtension);
            return clockings;
        }
    }
}
