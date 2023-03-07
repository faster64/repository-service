using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using AutoMapper;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications;

namespace KiotVietTimeSheet.Application.Queries.GetCommissionDetailsByCommissionIds
{
    public class GetCommissionDetailsByCommissionIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetCommissionDetailsByCommissionIdsQuery, List<CommissionDetailDto>>
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly ICommissionDetailReadOnlyRepository _commissionDetailReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetCommissionDetailsByCommissionIdsQueryHandler(
            IAuthService authService,
            ICommissionDetailReadOnlyRepository commissionDetailReadOnlyRepository,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IMapper mapper
        ) : base(authService)
        {
            _commissionDetailReadOnlyRepository = commissionDetailReadOnlyRepository;
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<CommissionDetailDto>> Handle(GetCommissionDetailsByCommissionIdsQuery request, CancellationToken cancellationToken)
        {
            var commissionIds = request.CommissionIds;
            var productIds = request.ProductIds;

            var commissions =
                await _commissionReadOnlyRepository.GetBySpecificationAsync(new FindCommissionByIdsSpec(commissionIds));

            var commissionDetails = await _commissionDetailReadOnlyRepository.GetBySpecificationAsync(
                new FindCommissionDetailByProductIdsSpec(productIds).And(
                    new FindCommissionDetailByCommissionIdsSpec(commissionIds)));

            var result = commissionDetails.Select(cm =>
            {
                var commissionDetailDto = _mapper.Map<CommissionDetailDto>(cm);
                commissionDetailDto.CommissionName = commissions.FirstOrDefault(c => c.Id == cm.CommissionId)?.Name;
                return commissionDetailDto;
            }).ToList();

            return result;
        }
    }
}

