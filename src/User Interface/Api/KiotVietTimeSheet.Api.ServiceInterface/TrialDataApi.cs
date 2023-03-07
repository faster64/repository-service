using KiotVietTimeSheet.Api.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Linq;
using Message = KiotVietTimeSheet.Resources.Message;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Application.Commands.CreateTrialData;
using KiotVietTimeSheet.Application.Commands.DeleteTrialData;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class TrialDataApi : BaseInternalApi
    {
        #region Properties
        private readonly IMediator _mediator;
        #endregion

        #region Constructors
        public TrialDataApi(
            ILogger<TrialDataApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }
        #endregion

        #region POST methods
        public async Task<object> Post(CreateBookingTrialDataReq req)
        {
            var returnObj = await _mediator.Send(new CreateTrialDataCommand(req.TrialType, req.BranchId, req.UserId1, req.UserId2, req.UserIdAdmin, req.TenantCode, req.TenantId, req.GroupId));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }
            return Ok(returnObj);
        }
        
        public async Task<object> Post(DeleteBookingTrialDataReq req)
        {
            var returnObj = await _mediator.Send(new DeleteTrialDataCommand(req.TrialType, req.BranchId, req.UserIdAdmin, req.TenantCode, req.TenantId, req.GroupId));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }
            return Ok(returnObj);
        }
        #endregion
    }
}
