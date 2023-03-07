using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;
using ServiceStack;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetClockingById
{
    public class GetClockingByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetClockingByIdQuery, ClockingDto>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;


        public GetClockingByIdQueryHandler(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IAuthService authService
            
        ) : base(authService)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
        }

        public async Task<ClockingDto> Handle(GetClockingByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _clockingReadOnlyRepository.FindByIdAsync(request.Id);
            return result.ConvertTo<ClockingDto>();
        }
    }
}
