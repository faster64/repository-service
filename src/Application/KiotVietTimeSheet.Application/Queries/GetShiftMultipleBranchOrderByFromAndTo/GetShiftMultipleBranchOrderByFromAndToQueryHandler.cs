using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetShiftMultipleBranchOrderByFromAndTo
{
    public class GetShiftMultipleBranchOrderByFromAndToQueryHandler : QueryHandlerBase,
        IRequestHandler<GetShiftMultipleBranchOrderByFromAndToQuery, List<ShiftDto>>
    {
        private readonly IMapper _mapper;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;

        public GetShiftMultipleBranchOrderByFromAndToQueryHandler(
            IMapper mapper,
            IAuthService authService,
            IShiftReadOnlyRepository shiftReadOnlyRepository

        ) : base(authService)
        {
            _mapper = mapper;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
        }

        public async Task<List<ShiftDto>> Handle(GetShiftMultipleBranchOrderByFromAndToQuery request, CancellationToken cancellationToken)
        {
            var result = await _shiftReadOnlyRepository.GetShiftMultipleBranchOrderByFromAndTo(request.BranchIds, request.ShiftIds,
                request.IncludeShiftIds, request.IncludeDeleted);
            result = result.OrderBy(s => s.From).ThenBy(s => s.From <= s.To ? s.To - s.From : 24 * 60 + s.To - s.From).ThenByDescending(s => s.CreatedDate).ToList();
            return _mapper.Map<List<ShiftDto>>(result);
        }
    }
}
