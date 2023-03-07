
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

namespace KiotVietTimeSheet.Application.Queries.GetPenalizesByTenantId
{
    public class GetPenalizesByIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPenalizesByIdsQuery, List<PenalizeDto>>
    {
        private readonly IPenalizeReadOnlyRepository _penalizeReadOnlyRepository;
        private readonly IMapper _mapper;
        public GetPenalizesByIdsQueryHandler(
            IPenalizeReadOnlyRepository penalizeReadOnlyRepository,
            IAuthService authService,
            IMapper mapper
        ) : base(authService)
        {
            _penalizeReadOnlyRepository = penalizeReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<PenalizeDto>> Handle(GetPenalizesByIdsQuery request, CancellationToken cancellationToken)
        {
            var result = await _penalizeReadOnlyRepository.GetBySpecificationAsync(new FindPenalizeByIdsSpec(request.PenalizeId), false, true);

            return _mapper.Map<List<PenalizeDto>>(result);
        }
    }
}
