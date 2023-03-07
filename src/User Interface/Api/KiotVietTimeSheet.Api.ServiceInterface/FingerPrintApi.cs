using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Commands.CreateUpdateFingerPrint;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByCurrentUserId;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Queries.GetFingerPrint;
using KiotVietTimeSheet.Application.Queries.GetFingerPrintByEmployeeId;
using KiotVietTimeSheet.Application.Queries.GetFingerPrintByFingerCode;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;
using Message = KiotVietTimeSheet.Resources.Message;


namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class FingerPrintApi : BaseApi
    {
        public IAutoQueryDb AutoQuery { get; set; }
        private readonly IMediator _mediator;

        public FingerPrintApi(ILogger<FingerPrintApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
            ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Get(GetListFingerPrintReq req)
        {
            var userInfo = await _mediator.Send(new GetEmployeeByCurrentUserIdQuery(false, false));
            if (userInfo != null && req.BranchId == null)
            {
                var branchIds = userInfo.EmployeeBranches.Select(x => x.BranchId).ToList();
                req.BranchIds = branchIds;
            }
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetFingerPrintQuery(query));
            return Ok(result);
        }

        public async Task<object> Get(GetFingerPrintByCodeReq req)
        {
            var result = await _mediator.Send(new GetFingerPrintByFingerCodeQuery(req.FingerCode, req.BranchId));
            return Ok(result);
        }

        public async Task<object> Get(GetFingerPrintByEmployeeReq req)
        {
            var result = await _mediator.Send(new GetFingerPrintByEmployeeIdQuery(req.employeeId, req.branchId));
            return Ok(result);
        }

        public async Task<object> Post(CreateUpdateFingerPrintReq req)
        {
            var result = await _mediator.Send(new CreateUpdateFingerPrintCommand(req.FingerPrint));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(result);

        }
    }
}
