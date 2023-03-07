using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Application.Service.Interfaces;
using ServiceStack;
using ServiceStack.Web;

namespace KiotVietTimeSheet.Api.ServiceInterface.Attributes
{
    /// <summary>
    /// Filter request API TimeSheet : trả về lỗi khi chức năng Chấm công, tính lương không đc bật hoặc cho phép gọi một số Api khi tắt chức năng Chấm công, tính lương
    /// </summary>
    public class CheckActiveTimeSheetFilterAttribute : RequestFilterAsyncAttribute
    {
        public override async Task ExecuteAsync(IRequest req, IResponse res, object requestDto)
        {
            if (requestDto is ClearCacheTimeSheet)
            {
                return;
            }
            var authService = HostContext.Resolve<IAuthService>();
            var posParamService = HostContext.Resolve<IPosParamService>();
            var appConfig = HostContext.Resolve<IApplicationConfiguration>();
            if (authService != null && appConfig != null)
            {
                var timeSheetPosParam =
                    await posParamService.GetTimeSheetPosParam(authService.Context.TenantId,
                        authService.Context.TenantCode);
                var allowApi = appConfig.ClassApiCanCallWhenInactiveTimeSheets;
                var allowToCall = false;
                if (allowApi != null && allowApi.Any())
                {
                    allowToCall = allowApi.Any(w => req.AbsoluteUri.Contains(w));
                }
                if (!timeSheetPosParam.IsActive && !allowToCall)
                {
                    throw new KvTimeSheetModuleUnActiveException();
                }
            }
        }
    }
}