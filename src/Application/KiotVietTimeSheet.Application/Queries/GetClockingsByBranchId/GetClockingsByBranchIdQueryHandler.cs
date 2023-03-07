using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetClockingsByBranchId
{
    public class GetClockingsByBranchIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetClockingsByBranchIdQuery, List<ClockingDto>>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetClockingsByBranchIdQueryHandler(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IAuthService authService,
            IMapper mapper
        ) : base(authService)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<ClockingDto>> Handle(GetClockingsByBranchIdQuery request, CancellationToken cancellationToken)
        {
            var specification = (new FindClockingByBranchIdSpec(request.BranchId)).Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void));
            var obj = await _clockingReadOnlyRepository.GetBySpecificationAsync(specification);
            var result = _mapper.Map<List<ClockingDto>>(obj);
            return result;
        }
    }
}
