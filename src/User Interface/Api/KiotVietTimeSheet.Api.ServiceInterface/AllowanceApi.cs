using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Commands.CreateAllowance;
using KiotVietTimeSheet.Application.Commands.DeleteAllowance;
using KiotVietTimeSheet.Application.Commands.UpdateAllowance;
using KiotVietTimeSheet.Application.Queries.GetAllowances;
using KiotVietTimeSheet.Application.Queries.GetAllowanceById;
using KiotVietTimeSheet.Application.Queries.GetAllowancesByIds;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class AllowanceApi : BaseApi
    {
        private readonly IMediator _mediator;

        public IAutoQueryDb AutoQuery { get; set; }

        public AllowanceApi(
            ILogger<AllowanceApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        )
            : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Post(CreateAllowanceReq req)
        {
            var result = await _mediator.Send(new CreateAllowanceCommand(req.Allowance));
            if (Errors.Any())
            {
                var message = GetFirstErrorMessage(Errors, Message.error_whenCreateData);
                return BadRequest(message, Errors);
            }

            return Ok(result);
        }

        public async Task<object> Put(UpdateAllowanceReq req)
        {
            var result = await _mediator.Send(new UpdateAllowanceCommand(req.Allowance));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(result);
        }

        public async Task<object> Delete(DeleteAllowanceReq req)
        {
            await _mediator.Send(new DeleteAllowanceCommand(req.Id));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.allowance.ToLower()));
        }

        public async Task<object> Get(GetListAllowanceReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetAllowancesQuery(query));
            return Ok(result);
        }

        public async Task<object> Get(GetAllowanceByIdReq req)
        {
            var result = await _mediator.Send(new GetAllowanceByIdQuery(req.Id));
            return result;
        }

        public async Task<object> Get(GetListAllowanceByIdsReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetAllowancesByIdsQuery(query));
            return result;
        }


    }
}
