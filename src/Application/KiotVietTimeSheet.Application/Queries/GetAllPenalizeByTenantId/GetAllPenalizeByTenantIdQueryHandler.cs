
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetAllPenalizeByTenantId
{
    public class GetAllPenalizeByTenantIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllPenalizeByTenantIdQuery, List<PenalizeDto>>
    {
        private readonly IPenalizeReadOnlyRepository _penalizeReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetAllPenalizeByTenantIdQueryHandler(
            IPenalizeReadOnlyRepository penalizeReadOnlyRepository,
            IMapper mapper,
            IAuthService authService
        ) : base(authService)
        {
            _penalizeReadOnlyRepository = penalizeReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<PenalizeDto>> Handle(GetAllPenalizeByTenantIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _penalizeReadOnlyRepository.GetBySpecificationAsync(new FindPenalizeByTenantIdSpec(AuthService.Context.TenantId), false, true);
            return _mapper.Map<List<PenalizeDto>>(result);
        }
    }
}
