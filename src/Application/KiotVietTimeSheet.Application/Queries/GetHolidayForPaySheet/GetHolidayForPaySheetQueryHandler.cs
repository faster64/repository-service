using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Queries.GetHolidayForPaySheet
{
    public class GetHolidayForPaySheetQueryHandler : QueryHandlerBase,
        IRequestHandler<GetHolidayForPaySheetQuery, List<HolidayDto>>
    {
        private readonly IHolidayReadOnlyRepository _holidayReadOnlyRepository;

        public GetHolidayForPaySheetQueryHandler(
            IAuthService authService,
            IHolidayReadOnlyRepository holidayReadOnlyRepository) : base(authService)
        {
            _holidayReadOnlyRepository = holidayReadOnlyRepository;
        }

        public async Task<List<HolidayDto>> Handle(GetHolidayForPaySheetQuery request, CancellationToken cancellationToken)
        {
            var result = await _holidayReadOnlyRepository.GetBySpecificationAsync((new FindHolidayByFromGreaterThanOrEqualSpec(request.From).And(new FindHolidayByFromLessThanOrEqualSpec(request.To)))
                .Or(new FindHolidayByToGreaterThanOrEqualSpec(request.From).And(new FindHolidayByToLessThanOrEqualSpec(request.To))));
            return result.ConvertTo<List<HolidayDto>>();
        }
    }
}
