using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetShiftByWorkingDayEmployee
{
    public class GetShiftByWorkingDayEmployeeQueryHandler : QueryHandlerBase,
        IRequestHandler<GetShiftByWorkingDayEmployeeQuery, List<ShiftDto>>
    {
        private readonly IMapper _mapper;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IClockingReadOnlyRepository _clockingRepository;
        public GetShiftByWorkingDayEmployeeQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IClockingReadOnlyRepository clockingRepository

        ) : base(authService)
        {
            _mapper = mapper;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _clockingRepository = clockingRepository;
        }

        public async Task<List<ShiftDto>> Handle(GetShiftByWorkingDayEmployeeQuery request, CancellationToken cancellationToken)
        {
            var findClockingByWorkByIdSpec = (new FindClockingByWorkByIdSpec(request.EmployeeId))
                .And(new FindClockingByStartTimeGreaterThanOrEqualSpec(request.StartTime.Date))
                .And(new FindClockingByStartTimeLessThanSpec(request.StartTime.Date.AddDays(1)));
            var clockings = await _clockingRepository.GetBySpecificationAsync(findClockingByWorkByIdSpec);
            var shiftIds = clockings.Select(x => x.ShiftId).ToList();
            var findShiftByShiftIdsSpec = new FindShiftByShiftIdsSpec(shiftIds);
            var shifts = await _shiftReadOnlyRepository.GetBySpecificationAsync(findShiftByShiftIdsSpec);
            var result = _mapper.Map<List<ShiftDto>>(shifts);
            return result;
        }
    }
}
