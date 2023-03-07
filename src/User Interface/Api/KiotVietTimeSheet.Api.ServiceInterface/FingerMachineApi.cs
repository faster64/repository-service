using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Commands.CreateFingerMachine;
using KiotVietTimeSheet.Application.Commands.DeleteFingerMachine;
using KiotVietTimeSheet.Application.Commands.UpdateFingerMachine;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Queries.GetFingerMachine;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class FingerMachineApi : BaseApi
    {
        public IAutoQueryDb AutoQuery { get; set; }
        private readonly IMediator _mediator;

        public FingerMachineApi(
            ILogger<FingerMachineApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Get(GetListFingerMachineReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetFingerMachineQuery(query));
            return Ok(result);
        }

        public async Task<object> Post(CreateFingerMachineReq req)
        {
            var result = await _mediator.Send(new CreateFingerMachineCommand(req.FingerMachine));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(result);
        }

        public async Task<object> Delete(DeleteFingerMachineReq req)
        {
            await _mediator.Send(new DeleteFingerMachineCommand(req.Id));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.fingerMachine.ToLower()));

        }

        public async Task<object> Put(UpdateFingerMachineReq req)
        {
            var result = await _mediator.Send(new UpdateFingerMachineCommand(req.FingerMachine));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(result);
        }
    }
}
