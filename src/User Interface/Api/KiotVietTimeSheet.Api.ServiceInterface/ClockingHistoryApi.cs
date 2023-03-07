using KiotVietTimeSheet.Api.ServiceModel;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Queries.GetAllClockingHistory;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class ClockingHistoryApi : BaseApi
    {
        #region Properties
        private readonly IMediator _mediator;
        public IAutoQueryDb AutoQuery { get; set; }
        #endregion

        public ClockingHistoryApi(
            ILogger<ClockingHistoryApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Get(GetListClockingHistoryReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            return Ok(await _mediator.Send(new GetAllClockingHistoryQuery(query)));
        }


    }
}
