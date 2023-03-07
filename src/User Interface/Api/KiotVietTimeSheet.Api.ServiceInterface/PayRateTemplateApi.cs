using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Commands.CopyPayrateTemplate;
using KiotVietTimeSheet.Application.Commands.CreatePayrateTemplate;
using KiotVietTimeSheet.Application.Commands.DeletePayrateTemplate;
using KiotVietTimeSheet.Application.Commands.UpdatePayrateTemplate;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Queries.GetPayrateTemplate;
using KiotVietTimeSheet.Application.Queries.GetPayrateTemplateById;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class PayRateTemplateApi : BaseApi
    {
        public IAutoQueryDb AutoQuery { get; set; }
        private readonly IMediator _mediator;
        public PayRateTemplateApi(
            ILogger<PayRateTemplateApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator

            ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Get(GetPayRateTemplateReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetPayrateTemplateQuery(query));
            return Ok(result);
        }

        public async Task<object> Get(GetPayRateTemplateByIdReq req)
        {
            var result = await _mediator.Send(new GetPayrateTemplateByIdQuery(req.Id));
            return Ok(result);
        }

        public async Task<object> Post(CreatePayRateTemplateReq req)
        {
            var result = await _mediator.Send(new CreatePayrateTemplateCommand(req.PayRateTemplate, req.BranchId, req.IsGeneralSetting));
            if (Errors.Any()) return BadRequest(Message.error_whenCreateData, Errors);
            return Ok(result);
        }

        public async Task<object> Post(CopyPayRateTemplateReq req)
        {
            var result = await _mediator.Send(new CopyPayrateTemplateCommand(req.Id, req.Name));
            return Ok(result);
        }

        public async Task<object> Put(UpdatePayRateTemplateReq req)
        {
            var returnObj = await _mediator.Send(new UpdatePayrateTemplateCommand(req.PayRateTemplate, req.Id, req.UpdatePayRate, req.IsGeneralSetting));
            if (Errors.Any()) return BadRequest(Message.error_whenUpdateData, Errors);

            return Ok(returnObj);
        }

        public async Task<object> Delete(DeletePayRateTemplateReq req)
        {
            await _mediator.Send(new DeletePayrateTemplateCommand(req.Id, req.IsGeneralSetting));

            if (Errors.Any()) return BadRequest(Message.error_whenUpdateData, Errors);

            return Ok(string.Format(Message.delete_successed, Label.paysheet_type.ToLower()));
        }
    }
}
