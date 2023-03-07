using KiotVietTimeSheet.Api.ServiceModel;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Commands.CreateDepartment;
using KiotVietTimeSheet.Application.Commands.DeleteDepartment;
using KiotVietTimeSheet.Application.Commands.UpdateDepartment;
using KiotVietTimeSheet.Application.Queries.GetDepartment;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class DepartmentApi : BaseApi
    {
        #region Properties
        public IAutoQueryDb AutoQuery { get; set; }
        private readonly IMediator _mediator;
        #endregion

        #region Constructors
        public DepartmentApi(
            ILogger<DepartmentApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }
        #endregion

        #region GET methods
        public async Task<object> Get(GetListDepartmentReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetDepartmentQuery(query));

            return Ok(result);
        }
        #endregion

        #region POST methods
        public async Task<object> Post(CreateDepartmentReq req)
        {
            var returnObj = await _mediator.Send(new CreateDepartmentCommand(req.Department));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }
        #endregion

        #region PUT methods
        public async Task<object> Put(UpdateDepartmentReq req)
        {
            await _mediator.Send(new UpdateDepartmentCommand(req.Department));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.department.ToLower()));
        }
        #endregion

        #region DELETE methods
        public async Task<object> Delete(DeleteDepartmentReq req)
        {
            await _mediator.Send(new DeleteDepartmentCommand(req.Id));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.department.ToLower()));
        }
        #endregion
    }
}
