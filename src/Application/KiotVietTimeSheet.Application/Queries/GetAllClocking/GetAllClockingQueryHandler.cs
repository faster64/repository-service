using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetAllClocking
{
    public class GetAllClockingQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllClockingQuery, PagingDataSource<ClockingDto>>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;

        public GetAllClockingQueryHandler(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IAuthService authService
        ) : base(authService)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
        }

        public async Task<PagingDataSource<ClockingDto>> Handle(GetAllClockingQuery request, CancellationToken cancellationToken)
        {
            var result = await _clockingReadOnlyRepository.FiltersAsync(request.Query);
            return result;
        }
    }

}
