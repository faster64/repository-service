using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Commands.CreatePenalize;
using KiotVietTimeSheet.Application.Commands.DeletePenalizes;
using KiotVietTimeSheet.Application.Commands.UpdatePenalize;
using KiotVietTimeSheet.Application.Queries.GetAllPenalizes;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class PenalizeApi : BaseApi
    {
        private readonly IMediator _mediator;

        public IAutoQueryDb AutoQuery { get; set; }

        public PenalizeApi(
            ILogger<PenalizeApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Get(GetAllPenalizeReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetAllPenalizesQuery(query));
            return Ok(result);
        }

        public async Task<object> Post(CreatePenalizesReq req)
        {

            var result = await _mediator.Send(new CreatePenalizeCommand(req.Penalize));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(result);
        }

        public async Task<object> Put(UpdatePenalizesReq req)
        {

            var result = await _mediator.Send(new UpdatePenalizeCommand(req.Penalize));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(result);
        }

        public async Task<object> Delete(DeletePenalizesReq req)
        {
            await _mediator.Send(new DeletePenalizesCommand(req.Id));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.penalize.ToLower()));
        }
    }
}
