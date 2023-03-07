using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Queries.GetHolidayById
{
    public class GetHolidayByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetHolidayByIdQuery, HolidayDto>
    {
        private readonly IHolidayReadOnlyRepository _holidayReadOnlyRepository;

        public GetHolidayByIdQueryHandler(
            IAuthService authService,
            IHolidayReadOnlyRepository holidayReadOnlyRepository) : base(authService)
        {
            _holidayReadOnlyRepository = holidayReadOnlyRepository;
        }

        public async Task<HolidayDto> Handle(GetHolidayByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _holidayReadOnlyRepository.FindByIdAsync(request.Id);
            return result.ConvertTo<HolidayDto>();
        }
    }
}
