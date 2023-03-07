using KiotVietTimeSheet.Application.Abstractions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;

namespace KiotVietTimeSheet.Application.Queries.GetHasAnyCommission
{
    public class GetHasAnyCommissionQueryHandler : QueryHandlerBase,
        IRequestHandler<GetHasAnyCommissionQuery, bool>
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IAuthService _authService;

        public GetHasAnyCommissionQueryHandler(
            IAuthService authService,
            ICommissionReadOnlyRepository commissionReadOnlyRepository
        ) : base(authService)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _authService = authService;
        }

        public async Task<bool> Handle(GetHasAnyCommissionQuery request, CancellationToken cancellationToken)
        {
            var commissions = await _commissionReadOnlyRepository.AnyBySpecificationAsync(new FindHasAnyCommissionByTenantIdSpec(_authService.Context.TenantId, request.IncludeDeleted));
            return commissions;
        }
    }
}

