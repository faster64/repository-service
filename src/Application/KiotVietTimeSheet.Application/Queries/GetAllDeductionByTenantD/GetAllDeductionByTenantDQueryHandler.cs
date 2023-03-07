using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetAllDeductionByTenantD
{
    public class GetAllDeductionByTenantDQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllDeductionByTenantDQuery, List<DeductionDto>>
    {
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetAllDeductionByTenantDQueryHandler(
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IMapper mapper,
            IAuthService authService
        ) : base(authService)
        {
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<DeductionDto>> Handle(GetAllDeductionByTenantDQuery request, CancellationToken cancellationToken)
        {
            var result = await _deductionReadOnlyRepository.GetBySpecificationAsync(new FindDeductionByTenantIdSpec(request.TenantId));
            return  _mapper.Map<List<DeductionDto>>(result);
        }
    }
}
