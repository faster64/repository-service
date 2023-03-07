
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetAllPenalizes
{
    public class GetAllPenalizesQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllPenalizesQuery, PagingDataSource<PenalizeDto>>
    {
        private readonly IPenalizeReadOnlyRepository _penalizeReadOnlyRepository;

        public GetAllPenalizesQueryHandler(
            IPenalizeReadOnlyRepository penalizeReadOnlyRepository,
            IAuthService authService
        ) : base(authService)
        {
            _penalizeReadOnlyRepository = penalizeReadOnlyRepository;
        }

        public async Task<PagingDataSource<PenalizeDto>> Handle(GetAllPenalizesQuery request, CancellationToken cancellationToken)
        {
            var result = await _penalizeReadOnlyRepository.FiltersAsync(request.Query, true);
            return result;
        }
    }
}
