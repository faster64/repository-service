using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetBranchsForMobile;
using KiotVietTimeSheet.Application.Queries.GetTimesheetConfigForMobile;
using KiotVietTimeSheet.Application.Queries.GetUserAccount;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class MobileApi : BaseApi
    {
        #region Properties

        private readonly IMediator _mediator;

        #endregion Properties

        #region Constructors

        public MobileApi(
            ILogger<MobileApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        #endregion Constructors

        #region GET methods

        public async Task<object> Get(GetTimesheetConfigForMobileReq _)
        {
            var returnObj = await _mediator.Send(new GetTimesheetConfigForMobileQuery());
            return Ok(returnObj);
        }

        public async Task<object> Get(GetBranchsForMobileReq _)
        {
            var returnObj = await _mediator.Send(new GetBranchsForMobileQuery());
            return Ok(returnObj);
        }

        public async Task<object> Get(GetUserMobileReq _)
        {
            try
            {
                var returnObj = await _mediator.Send(new GetUserAccountQuery()) ?? new UserAccountDto
                {
                    Language = "vi-VN"
                };
                return Ok(returnObj);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, e.Message);
                return Ok(e.Message, new UserAccountDto
                {
                    Language = "vi-VN"
                });
            }
        }

        #endregion GET methods
    }
}