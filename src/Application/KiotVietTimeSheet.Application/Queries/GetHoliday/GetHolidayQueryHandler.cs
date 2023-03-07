using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Queries.GetHoliday
{
    public class GetHolidayQueryHandler : QueryHandlerBase,
        IRequestHandler<GetHolidayQuery, HolidayPagingDataSource>
    {
        private readonly IHolidayReadOnlyRepository _holidayReadOnlyRepository;

        public GetHolidayQueryHandler(
            IAuthService authService,
            IHolidayReadOnlyRepository holidayReadOnlyRepository) : base(authService)
        {
            _holidayReadOnlyRepository = holidayReadOnlyRepository;
        }

        public async Task<HolidayPagingDataSource> Handle(GetHolidayQuery request, CancellationToken cancellationToken)
        {
            var dataSource = await _holidayReadOnlyRepository.FiltersAsync(request.Query);
            var result = dataSource.ConvertTo<HolidayPagingDataSource>();

            return result;
        }
    }
}
