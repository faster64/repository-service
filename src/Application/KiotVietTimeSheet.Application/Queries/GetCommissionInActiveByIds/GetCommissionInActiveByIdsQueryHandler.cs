using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using AutoMapper;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;

namespace KiotVietTimeSheet.Application.Queries.GetCommissionInActiveByIds
{
    public class GetCommissionInActiveByIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetCommissionInActiveByIdsQuery, List<CommissionDto>>
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetCommissionInActiveByIdsQueryHandler(
            IAuthService authService,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IMapper mapper
        ) : base(authService)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<CommissionDto>> Handle(GetCommissionInActiveByIdsQuery request, CancellationToken cancellationToken)
        {
            var ids = request.Ids;
            var specification = new FindCommissionInActiveByIdsSpec(ids);
            var result = await _commissionReadOnlyRepository.GetBySpecificationAsync(specification, true, true);
            return _mapper.Map<List<CommissionDto>>(result);
        }
    }
}

