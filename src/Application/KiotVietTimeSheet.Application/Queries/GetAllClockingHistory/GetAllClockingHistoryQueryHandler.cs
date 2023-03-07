using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetAllClockingHistory
{
    public class GetAllClockingHistoryQueryHandler : QueryHandlerBase,
    IRequestHandler<GetAllClockingHistoryQuery, PagingDataSource<ClockingHistoryDto>>
    {
        private readonly IClockingHistoryReadOnlyRepository _clockingHistoryReadOnlyRepository;

        public GetAllClockingHistoryQueryHandler(
            IClockingHistoryReadOnlyRepository clockingHistoryRepository,
            IAuthService authService
        ) : base(authService)
        {
            _clockingHistoryReadOnlyRepository = clockingHistoryRepository;
        }

        public async Task<PagingDataSource<ClockingHistoryDto>> Handle(GetAllClockingHistoryQuery request, CancellationToken cancellationToken)
        {
            var result = await _clockingHistoryReadOnlyRepository.FiltersAsync(request.Query);
            return result;
        }
    }
}
