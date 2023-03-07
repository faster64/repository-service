using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using AutoMapper;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;

namespace KiotVietTimeSheet.Application.Queries.GetByCommissionCurrentBranch
{
    public class GetByCommissionCurrentBranchQueryHandler : QueryHandlerBase,
        IRequestHandler<GetByCommissionCurrentBranchQuery, List<CommissionDto>>
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public GetByCommissionCurrentBranchQueryHandler(
            IAuthService authService,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IMapper mapper
        ) : base(authService)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<List<CommissionDto>> Handle(GetByCommissionCurrentBranchQuery request, CancellationToken cancellationToken)
        {
            var includeCommissionIds = request.IncludeCommissionIds;
            var includeInActive = request.IncludeInActive;
            var includeDeleted = request.IncludeDeleted;
            var commissions = await _commissionReadOnlyRepository.GetListForCurrentBranch(new CommissionQueryFilter
            {
                BranchIds = new List<int>
                {
                    _authService.Context.BranchId
                },
                IncludeCommissionIds = includeCommissionIds,
                IncludeInActive = includeInActive,
                IncludeIsDeleted = includeDeleted
            });

            return _mapper.Map<List<Commission>, List<CommissionDto>>(commissions);
        }
    }
}

