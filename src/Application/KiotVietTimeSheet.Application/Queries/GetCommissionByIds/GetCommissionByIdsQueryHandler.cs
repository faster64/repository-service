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

namespace KiotVietTimeSheet.Application.Queries.GetCommissionByIds
{
    public class GetCommissionByIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetCommissionByIdsQuery, List<CommissionDto>>
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetCommissionByIdsQueryHandler(
            IAuthService authService,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IMapper mapper
        ) : base(authService)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<CommissionDto>> Handle(GetCommissionByIdsQuery request, CancellationToken cancellationToken)
        {
            var ids = request.Ids;
            var specification = new FindActiveCommissionByIdsSpec(ids);
            var result = await _commissionReadOnlyRepository.GetBySpecificationAsync(specification, true, true);
            return _mapper.Map<List<CommissionDto>>(result);
        }
    }
}
