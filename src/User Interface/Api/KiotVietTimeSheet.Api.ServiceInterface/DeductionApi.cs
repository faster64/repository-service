using KiotVietTimeSheet.Api.ServiceModel;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Commands.CreateDeduction;
using KiotVietTimeSheet.Application.Commands.DeleteDeduction;
using KiotVietTimeSheet.Application.Commands.UpdateDeduction;
using KiotVietTimeSheet.Application.Queries.GetAllDeduction;
using KiotVietTimeSheet.Application.Queries.GetDeductionById;
using KiotVietTimeSheet.Application.Queries.GetDeductionByIds;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class DeductionApi : BaseApi
    {
        private readonly IMediator _mediator;

        public IAutoQueryDb AutoQuery { get; set; }

        public DeductionApi(
               ILogger<DeductionApi> logger,
               INotificationHandler<DomainNotification> notificationHandler,
               IMediator mediator
        )
            : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Post(CreateDeductionReq req)
        {
            var result = await _mediator.Send(new CreateDeductionCommand(req.Deduction, req.DeductionDetail));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(result);
        }

        public async Task<object> Put(UpdateDeductionReq req)
        {
            var result = await _mediator.Send(new UpdateDeductionCommand(req.Deduction, req.DeductionDetail));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(result);
        }

        public async Task<object> Delete(DeleteDeductionReq req)
        {
            await _mediator.Send(new DeleteDeductionCommand(req.Id));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.deduction.ToLower()));
        }

        public async Task<object> Get(GetListDeductionReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetAllDeductionQuery(query));
            return Ok(result);
        }

        public async Task<object> Get(GetDeductionByIdReq req)
        {
            var result = await _mediator.Send(new GetDeductionByIdQuery(req.Id));
            return result;
        }

        public async Task<object> Get(GetListDeductionByIdsReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetDeductionByIdsQuery(query));
            return result;
        }
    }
}
