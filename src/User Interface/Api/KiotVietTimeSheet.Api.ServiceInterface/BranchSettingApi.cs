using KiotVietTimeSheet.Api.ServiceModel;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using Message = KiotVietTimeSheet.Resources.Message;
using KiotVietTimeSheet.Application.Commands.CreateUpdateBranchSetting;
using KiotVietTimeSheet.Application.Queries.GetBranchSetting;
using KiotVietTimeSheet.Application.Queries.GetBranchSettingByIds;
using KiotVietTimeSheet.Application.Queries.CheckCanUpdateBranchSetting;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class BranchSettingApi : BaseApi
    {
        private readonly IMediator _mediator;

        public BranchSettingApi(
            ILogger<BranchSettingApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        )
            : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Get(GetBranchSettingReq req)
        {
            var branchSetting = await _mediator.Send(new GetBranchSettingByIdQuery(req.BranchId));
            return Ok(branchSetting);
        }

        public async Task<object> Get(GetBranchSettingByBranchIdsReq req)
        {
            var branchSetting = await _mediator.Send(new GetBranchSettingByIdsQuery(req.BranchIds));
            return Ok(branchSetting);
        }

        public async Task<object> Post(CreateOrUpdateBranchSettingReq req)
        {
            var branchSetting = await _mediator.Send(new CreateOrUpdateBranchSettingCommand(req.BranchSetting, req.IsAddMore, req.IsRemove, req.ApplyFrom));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.branch_day.ToLower()), branchSetting);
        }

        public async Task<object> Post(CheckCanUpdateBranchSettingReq req)
        {
            var result = await _mediator.Send(new CheckCanUpdateBranchSettingQuery(req.BranchSetting, req.IsAddMore,
                req.IsRemove, req.ApplyFrom));
            if (Errors.Any())
            {
                return Ok(result, Errors.FirstOrDefault()?.Message);
            }

            return Ok(result, Message.branch_canUpdate);
        }
    }
}
