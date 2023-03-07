using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetAllowances
{
    public class GetAllowancesQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllowancesQuery, PagingDataSource<AllowanceDto>>
    {
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;

        public GetAllowancesQueryHandler(
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IAuthService authService
        ) : base(authService)
        {
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
        }

        public async Task<PagingDataSource<AllowanceDto>> Handle(GetAllowancesQuery request, CancellationToken cancellationToken)
        {
            var result = await _allowanceReadOnlyRepository.FiltersAsync(request.Query);
            return result;
        }
    }
}
