using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Commands.CreateFingerPrintLog;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class FingerPrintLogApi : BaseApi
    {
        private readonly IMediator _mediator;

        public FingerPrintLogApi(
            ILogger<FingerPrintLogApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Post(CreateFingerPrintLogReq req)
        {
            var result = await _mediator.Send(new CreateFingerPrintLogCommand(req.FingerPrintLogs));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(result);
        }
    }
}
