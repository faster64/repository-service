using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetClockingsForCalendar
{
    public class GetClockingsForCalendarQueryHandler : QueryHandlerBase,
        IRequestHandler<GetClockingsForCalendarQuery, PagingDataSource<ClockingDto>>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;

        public GetClockingsForCalendarQueryHandler(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IAuthService authService 
        ) : base(authService)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
        }

        public async Task<PagingDataSource<ClockingDto>> Handle(GetClockingsForCalendarQuery request, CancellationToken cancellationToken)
        {
            var clockings = await _clockingReadOnlyRepository.GetListClockingForCalendar(request.Query, request.ClockingHistoryStates, request.DepartmentIds);
            return clockings;
        }
    }
}
