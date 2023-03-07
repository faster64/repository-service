using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Commands.CreateCommission;
using KiotVietTimeSheet.Application.Commands.DeleteCommission;
using KiotVietTimeSheet.Application.Commands.UpdateCommission;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Queries.GetByCommissionCurrentBranch;
using KiotVietTimeSheet.Application.Queries.GetComissionAll;
using KiotVietTimeSheet.Application.Queries.GetCommissionById;
using KiotVietTimeSheet.Application.Queries.GetCommissionByIds;
using KiotVietTimeSheet.Application.Queries.GetCommissionInActiveByIds;
using KiotVietTimeSheet.Application.Queries.GetHasAnyCommission;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;
using KiotVietTimeSheet.Application.Queries.GetCommissionByName;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class CommissionApi : BaseApi
    {
        #region Properties
        public IAutoQueryDb AutoQuery { get; set; }
        private readonly IMediator _mediator;
        #endregion

        #region Constructors
        public CommissionApi(
            ILogger<CommissionApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }
        #endregion

        #region GET methods
        public async Task<object> Get(GetListCommissionReq req)
        {
            return Ok(await _mediator.Send(new GetComissionAllQuery(req.Keyword, req.IsActive.GetValueOrDefault())));
        }

        public async Task<object> Get(GetListCommissionCurrentBranchReq req)
        {
            var result = await _mediator.Send(new GetByCommissionCurrentBranchQuery(req.IncludeDeleted,
                    req.IncludeInActive, req.IncludeCommissionIds));
            return Ok(result);
        }

        public async Task<object> Get(GetHasAnyCommissionReq req)
        {
            var result = await _mediator.Send(new GetHasAnyCommissionQuery(req.IncludeDeleted));
            return Ok(new { HasCommission = result });
        }

        public async Task<object> Get(GetListCommissionByIdsReq req)
        {
            if (req.CheckInActive)
                return await _mediator.Send(new GetCommissionInActiveByIdsQuery(req.Ids));

            return await _mediator.Send(new GetCommissionByIdsQuery(req.Ids));
        }

        public async Task<object> Get(GetCommissionTableByIdReq req)
        {
            var result = await _mediator.Send(new GetCommissionByIdQuery(req.Id));
            return result;
        }

        public async Task<object> Get(GetCommissionByNamesReq req)
        {
            var result = await _mediator.Send(new GetCommissionByNameQuery(req.Names));
            return result;
        }

        #endregion

        #region POST methods
        public async Task<object> Post(CreateCommissionReq req)
        {
            var returnObj = await _mediator.Send(new CreateCommissionCommand(req.Commission));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }
        #endregion

        #region PUT methods
        public async Task<object> Put(UpdateCommissionReq req)
        {
            await _mediator.Send(new UpdateCommissionCommand(req.Commission));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.commission.ToLower()));
        }
        #endregion

        #region DELETE methods
        public async Task<object> Delete(DeleteCommissionReq req)
        {
            await _mediator.Send(new DeleteCommissionCommand(req.Id));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.commission));
        }
        #endregion
    }
}
