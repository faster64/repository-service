using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Queries.GetListGpsInfo;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.SharedKernel.Auth;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using static KiotVietTimeSheet.Api.ServiceModel.Login;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class LoginApi : BaseAnonymousApi
    {
        private readonly IMediator _mediator;
        private readonly IAppSettings _appSettings;
        private readonly IMasterDbService _masterDbService;

        public LoginApi(
            ILogger<BranchSettingApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator,
            IAppSettings appSettings,
            IMasterDbService masterDbService
        )
            : base(logger, notificationHandler)
        {
            _mediator = mediator;
            _appSettings = appSettings;
            _masterDbService = masterDbService;
        }

        public async Task<object> Post(LoginForClockingGpsReq req)
        {
            var retailer = await _masterDbService.GetRetailerAsync(req.TenantId);
            if (retailer == null || retailer.Code != req.TenantCode) return BadRequest("Mã QR không hợp lệ, Vui lòng quét lại.", new List<ErrorResult>());

            var gpsInfo = await _mediator.Send(new GetGpsInfoByQrCodeQuery { Tenant = new TenantModel { Id = req.TenantId, Code = req.TenantCode }, QrKey = req.QrKey });

            if (Errors.Any())
            {
                return BadRequest(Errors.FirstOrDefault().Message, Errors);
            }

            var jwtProvider = new JwtAuthProvider(_appSettings)
            {
                ExpireTokensIn = TimeSpan.FromMinutes(30)
            };

            var header = JwtAuthProvider.CreateJwtHeader(jwtProvider.HashAlgorithm);
            var body = JwtAuthProvider.CreateJwtPayload(new AuthUserSession
            {
                DisplayName = "Clocking Gps",
                IsAuthenticated = true,
            },
                issuer: jwtProvider.Issuer,
                expireIn: jwtProvider.ExpireTokensIn,
                roles: new[] { "Employee" },
                permissions: new[] { "ThePermission" });
            body.Add("kvrid", retailer.Id.ToString());
            body.Add("kvrcode", retailer.Code);
            body.Add("kvrgid", retailer.GroupId.ToString());
            body.Add("kvuadmin", "false");
            body.Add("kvrindid", retailer.IndustryId.ToString());
            body.Add("clockinggps", (new SessionClockingGps
            {
                LoginByClockingGps = true,
                Permissions = new List<string> { ClockingGpsPermission.Full }
            }).ToJson());

            var jwtToken = JwtAuthProvider.CreateJwt(header, body, jwtProvider.GetHashAlgorithm());

            return Ok(new { AccessToken = jwtToken, Retailer = retailer, BranchId = gpsInfo.BranchId, GpsInfo = new { gpsInfo.Coordinate, gpsInfo.RadiusLimit } });
        }
    }
}
