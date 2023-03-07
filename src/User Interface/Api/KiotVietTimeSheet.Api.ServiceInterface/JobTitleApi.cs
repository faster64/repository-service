using KiotVietTimeSheet.Api.ServiceModel;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Commands.CreateJobTitle;
using KiotVietTimeSheet.Application.Commands.DeleteJobTitle;
using KiotVietTimeSheet.Application.Commands.UpdateJobTitle;
using KiotVietTimeSheet.Application.Queries.GetJobTitle;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class JobTitleApi : BaseApi
    {
        #region Properties
        public IAutoQueryDb AutoQuery { get; set; }
        private readonly IMediator _mediator;
        #endregion

        #region Constructors
        public JobTitleApi(
            ILogger<JobTitleApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }
        #endregion

        #region GET methods
        public async Task<object> Get(GetListJobTitleReq req)
        {

            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            return Ok(await _mediator.Send(new GetJobTitleQuery(query)));
        }
        #endregion

        #region POST methods
        public async Task<object> Post(CreateJobTitleReq req)
        {
            var returnObj = await _mediator.Send(new CreateJobTitleCommand(req.JobTitle));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }
        #endregion

        #region PUT methods
        public async Task<object> Put(UpdateJobTitleReq req)
        {
            await _mediator.Send(new UpdateJobTitleCommand(req.JobTitle));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.jobTitle.ToLower()));

        }
        #endregion

        #region DELETE methods
        public async Task<object> Delete(DeleteJobTitleReq req)
        {
            await _mediator.Send(new DeleteJobTitleCommand(req.Id));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.jobTitle));
        }
        #endregion
    }
}
