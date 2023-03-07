using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetAllowanceByTenantId
{
    public class GetAllowanceByTenantIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllowanceByTenantIdQuery, List<AllowanceDto>>
    {
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetAllowanceByTenantIdQueryHandler(
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IMapper mapper,
            IAuthService authService
        ) : base(authService)
        {
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<AllowanceDto>> Handle(GetAllowanceByTenantIdQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<List<AllowanceDto>>(await _allowanceReadOnlyRepository.GetBySpecificationAsync(new FindAllowanceByTenantIdSpec(request.TenantId)));
        }
    }
}
