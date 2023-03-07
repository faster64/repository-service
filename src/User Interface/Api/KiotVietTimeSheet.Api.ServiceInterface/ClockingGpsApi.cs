using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Commands.UpdateClockingForClockingGps;
using KiotVietTimeSheet.Application.Commands.UpdateDeviceForEmployeeClockingGps;
using KiotVietTimeSheet.Application.Queries.GetClockingsForClockingGps;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByIdentityKeyForClockingGps;
using KiotVietTimeSheet.Application.Queries.GetEmployeeForClockingGps;
using KiotVietTimeSheet.Application.Queries.GetListGpsInfo;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class ClockingGpsApi : BaseApi
    {
        #region Properties
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        private readonly IMasterDbService _masterDbService;
        #endregion

        #region Constructors
        public ClockingGpsApi(
            ILogger<ClockingGpsApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator,
            IAuthService authService,
            IMasterDbService masterDbService
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
            _authService = authService;
            _masterDbService = masterDbService;
        }
        #endregion

        #region GET methods
        public async Task<object> Get(GetClockingsForClockingGpsReq req)
        {
            var returnObj = await _mediator.Send(new GetClockingsForClockingGpsQuery(req.BranchId, req.EmployeeId));
            return Ok(returnObj);
        }

        public async Task<object> Get(GetGpsInfoForClockingGpsReq req)
        {
            var tenantId = _authService.Context.TenantId;
            var retailer = await _masterDbService.GetRetailerAsync(tenantId);
            if (retailer == null || retailer.Code != _authService.Context.TenantCode) return BadRequest("Shop không hợp lệ.", new List<ErrorResult>());

            var gpsInfo = await _mediator.Send(new GetGpsInfoForClockingGpsQuery(req.BranchId));
            return Ok(new { Retailer = retailer, GpsInfo = new { gpsInfo.Coordinate, gpsInfo.RadiusLimit } });
        }

        #endregion

        #region POST methods

        public async Task<object> Get(GetEmployeeByIdentityKeyForClockingGpsReq req)
        {
            var returnObj = await _mediator.Send(new GetEmployeeByIdentityKeyForClockingGpsQuery(req.IdentityKeyClocking));

            if (Errors.Any())
            {
                return BadRequest(Resources.Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }

        public object Get(GetCurrentTimeServer req)
        {
            return DateTime.Now;
        }

        #endregion

        #region POST methods

        public async Task<object> Post(GetEmployeeForClockingGpsReq req)
        {
            var returnObj = await _mediator.Send(new GetEmployeeForClockingGpsQuery(
                req.IdentityKeyClocking,
                req.Os,
                req.OsVersion,
                req.Vendor,
                req.Model,
                req.Type,
                req.Keyword,
                req.IsPhone
            ));

            if (Errors.Any())
            {
                return BadRequest(Errors.FirstOrDefault()?.Message, Errors, returnObj);
            }
            return Ok(returnObj);
        }

        #endregion

        #region PUT methods

        public async Task<object> Put(UpdateDeviceForEmployeeClockingGpsReq req)
        {
            var returnObj = await _mediator.Send(new UpdateDeviceForEmployeeClockingGpsCommand(req.EmployeeId, req.VerifyCode, req.Os, req.OsVersion, req.Type));

            if (Errors.Any())
            {
                return BadRequest(Resources.Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Put(UpdateClockingForClockingGpsReq req)
        {
            var returnObj = await _mediator.Send(new UpdateClockingForClockingGpsCommand(req.Clocking, req.ClockingHistory, req.GeoCoordinate, req.IdentityKeyClocking, req.AcceptWrongGps));
            var message = "";
            if (Errors.Any())
            {
                return BadRequest(Resources.Message.error_whenUpdateData, Errors);
            }

            if (returnObj == null)
            {
                if (!req.AcceptWrongGps)
                    message = "Bạn vừa chấm công trên một thiết bị di động mới. Để được ghi nhận việc chấm công bạn cần báo với quản lý để xác nhận.";
                else
                    message = "Đã gửi yêu cầu xác nhận chấm công thành công.";
                return Ok(message);
            }

            return Ok(returnObj);
        }

        #endregion
    }
}
