using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Service.Interfaces;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetTimesheetConfigForMobile
{
    public class GetTimesheetConfigForMobileQueryHandler : QueryHandlerBase,
        IRequestHandler<GetTimesheetConfigForMobileQuery, object>
    {
        private readonly IPosParamService _posParamService;
        private readonly IAuthService _authService;

        public GetTimesheetConfigForMobileQueryHandler(
            IAuthService authService,
            IPosParamService posParamService

        ) : base(authService)
        {
            _posParamService = posParamService;
            _authService = authService;
        }

        public async Task<object> Handle(GetTimesheetConfigForMobileQuery request, CancellationToken cancellationToken)
        {
            var timesheetPosParam = await _posParamService.GetTimeSheetPosParam(_authService.Context.TenantId, _authService.Context.TenantCode);
            return new { IsUsing = timesheetPosParam.IsActive };
        }

    }
}
