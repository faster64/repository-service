using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetCommissionByName
{
    public class GetCommissionByNameQueryHandler : QueryHandlerBase,
        IRequestHandler<GetCommissionByNameQuery, List<CommissionDto>>
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly ICommissionBranchReadOnlyRepository _commissionBranchReadOnlyRepository;

        public GetCommissionByNameQueryHandler(
            IAuthService authService,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            ICommissionBranchReadOnlyRepository commissionBranchReadOnlyRepository
        ) : base(authService)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _commissionBranchReadOnlyRepository = commissionBranchReadOnlyRepository;
        }

        public async Task<List<CommissionDto>> Handle(GetCommissionByNameQuery request, CancellationToken cancellationToken)
        {
            var commission =
                await _commissionReadOnlyRepository.GetBySpecificationAsync(new FindCommissionByNamesSpec(request.Names), false,
                    true);
            var commissionBranch =
                await _commissionBranchReadOnlyRepository.GetBySpecificationAsync(
                    new FindCommissionBranchByCommissionIdsSpec(commission.Select(x => x.Id).ToList()));
            return commission.Select(c => new CommissionDto
            {
                Id = c.Id,
                IsAllBranch = c.IsAllBranch,
                Name = c.Name,
                IsDeleted = c.IsDeleted,
                IsActive = c.IsActive,
                BranchIds = commissionBranch.Where(b => b.CommissionId == c.Id).Select(b => b.BranchId).ToList()
            }).ToList();
        }
    }
}
