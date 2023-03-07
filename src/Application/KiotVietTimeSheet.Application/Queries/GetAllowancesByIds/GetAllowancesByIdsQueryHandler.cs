using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetAllowancesByIds
{
    public class GetAllowancesByIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllowancesByIdsQuery, PagingDataSource<AllowanceDto>>
    {
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;

        public GetAllowancesByIdsQueryHandler(
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IAuthService authService
        ) : base(authService)
        {
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
        }

        public async Task<PagingDataSource<AllowanceDto>> Handle(GetAllowancesByIdsQuery request, CancellationToken cancellationToken)
        {
            return await _allowanceReadOnlyRepository.FiltersAsync(request.Query, true);
        }
    }
}
