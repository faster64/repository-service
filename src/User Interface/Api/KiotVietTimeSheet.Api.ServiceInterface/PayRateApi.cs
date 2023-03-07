using KiotVietTimeSheet.Api.ServiceModel;
using System.Threading.Tasks;
using ServiceStack;
using Microsoft.Extensions.Logging;
using MediatR;
using KiotVietTimeSheet.Application.Queries.GetPayRate;
using KiotVietTimeSheet.Application.Queries.GetPayRateByEmployeeId;
using KiotVietTimeSheet.Application.Queries.GetPayRateById;
using KiotVietTimeSheet.SharedKernel.Notification;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class PayRateApi : BaseApi
    {

        public IAutoQueryDb AutoQuery { get; set; }
        private readonly IMediator _mediator;

        public PayRateApi(
            ILogger<PayRateApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Get(GetPayRateByEmployeeIdReq req)
        {
            var returnObj = await _mediator.Send(new GetPayRateByEmployeeIdQuery(req.EmployeeId));
            return Ok(returnObj);
        }

        public async Task<object> Get(GetPayRateByIdReq req)
        {
            return Ok(await _mediator.Send(new GetPayRateByIdQuery(req.Id)));
        }

        public async Task<object> Get(GetPayRateByTemplateIdReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetPayRateQuery(query));
            return Ok(result);
        }
    }
}
