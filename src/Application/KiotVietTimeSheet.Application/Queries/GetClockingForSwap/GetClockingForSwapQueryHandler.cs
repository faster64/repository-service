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

namespace KiotVietTimeSheet.Application.Queries.GetClockingForSwap
{
    public class GetClockingForSwapQueryHandler : QueryHandlerBase,
        IRequestHandler<GetClockingForSwapQuery, List<ClockingDto>>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetClockingForSwapQueryHandler(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IAuthService authService,
            IMapper mapper
        ) : base(authService)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<ClockingDto>> Handle(GetClockingForSwapQuery request, CancellationToken cancellationToken)
        {
            var day = request.Day;
            var shiftId = request.ShiftId;
            var employeeId = request.EmployeeId;
            var branchId = request.BranchId;
            var startDate = day.Date;
            var endDate = day.Date.AddDays(1);
            var getClockingForSwapSpec = new FindClockingByStatusSpec((byte)ClockingStatuses.Created)
                .And(new FindClockingByStartTimeGreaterThanOrEqualSpec(startDate))
                .And(new FindClockingByStartTimeLessThanSpec(endDate));

            if (shiftId > 0) getClockingForSwapSpec = getClockingForSwapSpec.And(new FindClockingByShiftIdSpec(shiftId));
            if (employeeId > 0) getClockingForSwapSpec = getClockingForSwapSpec.And(new FindClockingByWorkByIdSpec(employeeId));
            if (branchId > 0) getClockingForSwapSpec = getClockingForSwapSpec.And(new FindClockingByBranchIdSpec(branchId));

            var clockings = await _clockingReadOnlyRepository.GetBySpecificationAsync(getClockingForSwapSpec);
            var result = _mapper.Map<List<ClockingDto>>(clockings);
            return result;
        }
    }
}
