using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.ServiceClients;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetUserAccount
{
    public class GetUserAccountQueryHandler : QueryHandlerBase,
        IRequestHandler<GetUserAccountQuery, UserAccountDto>
    {
        private readonly IKiotVietServiceClient _kiotVietServiceClient;

        public GetUserAccountQueryHandler(
            IAuthService authService,
            IKiotVietServiceClient kiotVietServiceClient) : base(authService)
        {
            _kiotVietServiceClient = kiotVietServiceClient;
        }

        public async Task<UserAccountDto> Handle(GetUserAccountQuery request, CancellationToken cancellationToken)
        {
            return await _kiotVietServiceClient.GetUserAccount(request);
        }
    }
}