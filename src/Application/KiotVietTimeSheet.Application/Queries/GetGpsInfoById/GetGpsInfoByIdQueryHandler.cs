using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;
using ServiceStack;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetGpsInfoById
{
    public class GetGpsInfoByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetGpsInfoByIdQuery, GpsInfoDto>
    {
        private readonly IGpsInfoReadOnlyRepository _gpsInfoReadOnlyRepository;
        public GetGpsInfoByIdQueryHandler(
            IAuthService authService,
            IGpsInfoReadOnlyRepository gpsInfoReadOnlyRepository)
            :base(authService)
        {
            _gpsInfoReadOnlyRepository = gpsInfoReadOnlyRepository;
        }
        public async Task<GpsInfoDto> Handle(GetGpsInfoByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _gpsInfoReadOnlyRepository.FindByIdAsync(request.Id, true, true);
            return result.ConvertTo<GpsInfoDto>();
        }
    }
}
