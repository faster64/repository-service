using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using AutoMapper;
using KiotVietTimeSheet.Application.ServiceClients;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;

namespace KiotVietTimeSheet.Application.Queries.GetComissionAll
{
    public class GetComissionAllDataTrialQueryHandler : QueryHandlerBase,
        IRequestHandler<GetComissionAllDataTrialQuery, PagingDataSource<CommissionDto>>
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetComissionAllDataTrialQueryHandler(
            IAuthService authService,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IMapper mapper
        ) : base(authService)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<PagingDataSource<CommissionDto>> Handle(GetComissionAllDataTrialQuery request, CancellationToken cancellationToken)
        {
            var keyword = request.Keyword;
            var isActive = request.IsActive;
            keyword = string.IsNullOrEmpty(keyword) ? string.Empty : keyword;
            var listCommission = await _commissionReadOnlyRepository.GetAllWithinIncludeAsync(new[] { "CommissionBranches" });
            
            if (!string.IsNullOrEmpty(keyword))
            {
                listCommission = listCommission
                    .Where(x => x.Name.Contains(keyword))
                    .ToList();
            }

            if (isActive)
            {
                listCommission = listCommission
                    .Where(x => x.IsActive)
                    .ToList();
            }
            listCommission = listCommission
                .Where(x => x.CommissionBranches == null || x.CommissionBranches.Count == 0)
                .OrderBy(x => x.CreatedDate).ToList();
            var result = new PagingDataSource<CommissionDto>()
            {
                Data = _mapper.Map<List<Commission>, List<CommissionDto>>(listCommission),
                Total = listCommission.Count
            };
            return result;
        }
    }
}
