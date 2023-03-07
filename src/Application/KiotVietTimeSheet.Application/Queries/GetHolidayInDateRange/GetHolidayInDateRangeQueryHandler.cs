using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetHolidayInDateRange
{
    public class GetHolidayInDateRangeQueryHandler : QueryHandlerBase,
        IRequestHandler<GetHolidayInDateRangeQuery, List<HolidayDto>>
    {
        private readonly IHolidayReadOnlyRepository _holidayReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetHolidayInDateRangeQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IHolidayReadOnlyRepository holidayReadOnlyRepository) : base(authService)
        {
            _holidayReadOnlyRepository = holidayReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<HolidayDto>> Handle(GetHolidayInDateRangeQuery request, CancellationToken cancellationToken)
        {
            var spec = new FindHolidayByFromLessThanOrEqualSpec(request.EndTime)
                .And(new FindHolidayByToGreaterThanOrEqualSpec(request.StartTime));

            var result = await _holidayReadOnlyRepository.GetBySpecificationAsync(spec);

            return result.Select(h => _mapper.Map<HolidayDto>(h)).ToList();
        }
    }
}
