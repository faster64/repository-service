using KiotVietTimeSheet.Api.ServiceModel;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Commands.CreateOrUpdateSetting;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using Message = KiotVietTimeSheet.Resources.Message;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Commands.UpdateSettingClockingGps;
using ServiceStack;
using Microsoft.Extensions.Configuration;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Enum;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class SettingsApi : BaseApi
    {
        #region Properties
        private readonly IMediator _mediator;
        private readonly ICacheClient _cacheClient;
        #endregion

        #region Constructors
        public SettingsApi(
            ILogger<SettingsApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator,
            ICacheClient cacheClient
        )
            : base(logger, notificationHandler)
        {
            _mediator = mediator;
            _cacheClient = cacheClient;
        }

        public object Get(ClearCacheTimeSheet req)
        {
            if (req.TenantId > 0)
            {
                var posParamCacheKey = CacheKeys.GetTimeSheetPosParamCacheKey(req.TenantId);
                _cacheClient.Remove(posParamCacheKey);

                if (req.UserId > 0)
                {
                    var permissionKey = CacheKeys.GetPermissionMapCacheKey(req.TenantId, req.UserId);
                    _cacheClient.Remove(permissionKey);

                    var atLeastNecessaryPermissionKey = CacheKeys.GetAtLeastNecessaryPermissionCacheKey(req.TenantId, req.UserId);
                    _cacheClient.Remove(atLeastNecessaryPermissionKey);

                    if (req.ClearPermit == true)
                    {
                        var permit = CacheKeys.GetAuthorizedBranchCacheKey(req.TenantId, req.UserId);
                        _cacheClient.Remove(permit);
                    }
                }
            }

            return Ok("Time sheet clear cache successful");
        }

        public object Get(ViewPermitBranch req)
        {
            if (req.TenantId > 0 && req.UserId > 0)
            {
                var permit = CacheKeys.GetAuthorizedBranchCacheKey(req.TenantId, req.UserId);
                return _cacheClient.GetOrDefault<object>(permit) ?? "No data";
            }
            return null;
        }

        public object Get(GetGoogleMapApiKey req)
        {
            var configuration = HostContext.Resolve<IConfiguration>();
            return Ok<object>(new { Key = configuration.GetSection("GoogleMapsApiKey").Get<string>() });
        }
        #endregion

        #region GET methods
        public async Task<object> Get(GetSettingReq req)
        {
            var result = await _mediator.Send(new GetSettingQuery(req.TenantId));
            return Ok(result);
        }
        #endregion

        #region POST methods
        public async Task<object> Post(CreateOrUpdateSettingReq req)
        {
            var settings = await _mediator.Send(new CreateOrUpdateSettingCommand(req.Data, (byte)SettingType.Clocking));

            if (Errors.Any())
            {
                return BadRequest(Errors.FirstOrDefault()?.Message, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.branch_day.ToLower()), settings);
        }

        public async Task<object> Post(CreateOrUpdateClockingSettingReq req)
        {
            var settings = await _mediator.Send(new CreateOrUpdateClockingSettingCommand(req.Data, (byte)SettingType.Clocking));

            if (Errors.Any())
            {
                return BadRequest(Errors.FirstOrDefault()?.Message, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.branch_day.ToLower()), settings);
        }

        public async Task<object> Post(CreateOrUpdateTimesheetSettingReq req)
        {
            var settings = await _mediator.Send(new CreateOrUpdateTimeSheetSettingCommand(req.Data, (byte)SettingType.TimeSheet));

            if (Errors.Any())
            {
                return BadRequest(Errors.FirstOrDefault()?.Message, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.branch_day.ToLower()), settings);
        }

        public async Task<object> Post(CreateOrUpdateCommissionSettingReq req)
        {
            var settings = await _mediator.Send(new CreateOrUpdateCommissionSettingCommand(req.Data, (byte)SettingType.Commission));

            if (Errors.Any())
            {
                return BadRequest(Errors.FirstOrDefault()?.Message, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.branch_day.ToLower()), settings);
        }

        public async Task<object> Post(UpdateSettingUseClockingGpsReq req)
        {
            await _mediator.Send(new UpdateSettingClockingGpsCommand(req.UseClockingGps));

            if (Errors.Any())
            {
                return BadRequest(Errors.FirstOrDefault()?.Message, Errors);
            }

            return Ok(new { });
        }
        #endregion
    }
}