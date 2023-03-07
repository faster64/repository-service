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
    public class GetComissionAllQueryHandler : QueryHandlerBase,
        IRequestHandler<GetComissionAllQuery, PagingDataSource<CommissionDto>>
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;

        public GetComissionAllQueryHandler(
            IAuthService authService,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IMapper mapper,
            IKiotVietServiceClient kiotVietServiceClient
        ) : base(authService)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _mapper = mapper;
            _kiotVietServiceClient = kiotVietServiceClient;
        }

        public async Task<PagingDataSource<CommissionDto>> Handle(GetComissionAllQuery request, CancellationToken cancellationToken)
        {
            var keyword = request.Keyword;
            var isActive = request.IsActive;
            keyword = string.IsNullOrEmpty(keyword) ? string.Empty : keyword;
            var listCommission = await _commissionReadOnlyRepository.GetAllWithinIncludeAsync(new[] { "CommissionBranches" });
            var branchHasPermissionIds = (await _kiotVietServiceClient.GetBranchByPermission(TimeSheetPermission.Commission_Read)).Data.Select(x => x.Id);
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
                .Where(x => x.CommissionBranches == null || x.CommissionBranches.Count == 0 || branchHasPermissionIds.Any(id => x.CommissionBranches.Any(cb => cb.BranchId == id)))
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
