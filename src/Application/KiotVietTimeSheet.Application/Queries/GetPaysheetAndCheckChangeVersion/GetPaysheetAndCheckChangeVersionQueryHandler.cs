using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetAndCheckChangeVersion
{
    public class GetPaysheetAndCheckChangeVersionQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPaysheetAndCheckChangeVersionQuery, bool>
    {
        private readonly IDetectionChangePaysheetDomainService _detectionChangePaysheetDomainService;

        public GetPaysheetAndCheckChangeVersionQueryHandler(
            IAuthService authService,
            IDetectionChangePaysheetDomainService detectionChangePaysheetDomainService

        ) : base(authService)
        {
            _detectionChangePaysheetDomainService = detectionChangePaysheetDomainService;
        }

        public async Task<bool> Handle(GetPaysheetAndCheckChangeVersionQuery request, CancellationToken cancellationToken)
        {
            return await _detectionChangePaysheetDomainService.IsChangePaysheetWhenMakePaymentsAsync(request.Id, request.Version);
        }
    }
}
