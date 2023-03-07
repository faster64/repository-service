using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetHolidayTotalDays
{
    public class GetHolidayTotalDaysQueryHandler : QueryHandlerBase,
        IRequestHandler<GetHolidayTotalDaysQuery, int>
    {
        private readonly IHolidayReadOnlyRepository _holidayReadOnlyRepository;

        public GetHolidayTotalDaysQueryHandler(
            IAuthService authService,
            IHolidayReadOnlyRepository holidayReadOnlyRepository) : base(authService)
        {
            _holidayReadOnlyRepository = holidayReadOnlyRepository;
        }

        public async Task<int> Handle(GetHolidayTotalDaysQuery request, CancellationToken cancellationToken)
        {
            var result = await _holidayReadOnlyRepository.SingleAsync(request.Query);
            return result;
        }
    }
}
