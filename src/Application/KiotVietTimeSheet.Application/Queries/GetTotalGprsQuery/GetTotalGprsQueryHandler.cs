using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetTotalGprsQuery
{
    public class GetTotalGprsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetTotalGprsQuery, long>
    {
        private readonly IGpsInfoReadOnlyRepository _gpsInfoReadOnlyRepository;

        public GetTotalGprsQueryHandler(IAuthService authService, IGpsInfoReadOnlyRepository gpsInfoReadOnlyRepository) : base(authService)
        {
            _gpsInfoReadOnlyRepository = gpsInfoReadOnlyRepository;
        }

        public async Task<long> Handle(GetTotalGprsQuery request, CancellationToken cancellationToken)
        {
            return await _gpsInfoReadOnlyRepository.CountByExpression(x => x.TenantId == AuthService.Context.TenantId && !x.IsDeleted);
        }
    }
}