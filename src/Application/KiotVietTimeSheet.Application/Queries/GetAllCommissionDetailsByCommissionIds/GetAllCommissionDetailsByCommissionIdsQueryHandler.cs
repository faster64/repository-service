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

namespace KiotVietTimeSheet.Application.Queries.GetAllCommissionDetailsByCommissionIds
{
    public class GetAllCommissionDetailsByCommissionIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllCommissionDetailsByCommissionIdsQuery, List<CommissionDetailDto>>
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetAllCommissionDetailsByCommissionIdsQueryHandler(
            IAuthService authService,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IMapper mapper
        ) : base(authService)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<CommissionDetailDto>> Handle(GetAllCommissionDetailsByCommissionIdsQuery request, CancellationToken cancellationToken)
        {
            var commissionIds = request.CommissionIds;
            var commissionList =
                await _commissionReadOnlyRepository.GetBySpecificationAsync(new FindCommissionByIdsSpec(commissionIds), true, false);

            var result = new List<CommissionDetailDto>();
            commissionList.ForEach(commission =>
            {
                if (commission.CommissionDetails != null)
                {
                    var commissionDetailsDto = commission.CommissionDetails.Where(cm => !cm.IsDeleted).Select(cm =>
                    {
                        var commissionDetailDto = _mapper.Map<CommissionDetailDto>(cm);
                        commissionDetailDto.CommissionName = commissionList.FirstOrDefault(c => c.Id == cm.CommissionId)?.Name;
                        return commissionDetailDto;
                    }).ToList();

                    if (commissionDetailsDto.Any())
                        result.AddRange(commissionDetailsDto);
                }

            });

            return result;
        }
    }
}
