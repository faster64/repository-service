using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetShiftOrderByFromAndTo
{
    public class GetShiftOrderByFromAndToQueryHandler : QueryHandlerBase,
        IRequestHandler<GetShiftOrderByFromAndToQuery, List<ShiftDto>>
    {
        private readonly IMapper _mapper;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;

        public GetShiftOrderByFromAndToQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IShiftReadOnlyRepository shiftReadOnlyRepository

        ) : base(authService)
        {
            _mapper = mapper;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
        }

        public async Task<List<ShiftDto>> Handle(GetShiftOrderByFromAndToQuery request, CancellationToken cancellationToken)
        {
            ISpecification<Shift> specification = new FindShiftByBranchIdSpec(request.BranchId);
            if (request.ShiftIds != null)
            {
                specification = specification.And(new FindShiftByShiftIdsSpec(request.ShiftIds));
            }

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                specification = specification.And(new FindShiftContainsName(request.Keyword));
            }

            var result = await _shiftReadOnlyRepository.GetBySpecificationAsync(specification);
            result = result.OrderBy(s => s.From).ThenBy(s => s.From <= s.To ? s.To - s.From : 24 * 60 + s.To - s.From)
                .ThenByDescending(s => s.CreatedDate).ToList();

            return _mapper.Map<List<ShiftDto>>(result);
        }
    }
}
