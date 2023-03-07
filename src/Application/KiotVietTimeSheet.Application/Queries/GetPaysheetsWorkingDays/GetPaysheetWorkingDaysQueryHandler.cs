using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetsWorkingDays
{
    public class GetPaysheetWorkingDaysQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPaysheetWorkingDaysQuery, object>
    {
        private readonly IWorkingDayForPaysheetDomainService _workingDayForPaysheetDomainService;
        private readonly IAuthService _authService;

        public GetPaysheetWorkingDaysQueryHandler(
            IAuthService authService,
            IWorkingDayForPaysheetDomainService workingDayForPaysheetDomainService

        ) : base(authService)
        {
            _authService = authService;
            _workingDayForPaysheetDomainService = workingDayForPaysheetDomainService;
        }

        public async Task<object> Handle(GetPaysheetWorkingDaysQuery request, CancellationToken cancellationToken)
        {
            var result = await _workingDayForPaysheetDomainService.GetWorkingDayPaysheetAsync(_authService.Context.BranchId, request.StartTime, request.EndTime);
            return result;
        }
    }
}
