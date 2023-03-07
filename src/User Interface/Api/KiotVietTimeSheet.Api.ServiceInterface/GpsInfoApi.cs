using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Commands.CreateGpsInfo;
using KiotVietTimeSheet.Application.Commands.DeleteGpsInfo;
using KiotVietTimeSheet.Application.Commands.UpdateGpsInfo;
using KiotVietTimeSheet.Application.Commands.UpdateQrKey;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetGpsInfoById;
using KiotVietTimeSheet.Application.Queries.GetListGpsInfo;
using KiotVietTimeSheet.Application.Queries.GetTotalGprsQuery;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class GpsInfoApi : BaseApi
    {
        #region Properties
        private readonly IMediator _mediator;
        public IAutoQueryDb AutoQuery { get; set; }
        #endregion

        #region Constructors
        public GpsInfoApi(
            ILogger<GpsInfoApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }
        #endregion

        #region GET methods
        public async Task<object> Get(GetListGpsInfoReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            query.OrderByDescending(x => x.CreatedDate);
            var result = await _mediator.Send(new GetListGpsInfoQuery(query,req.WithDeleted));
            return Ok(result);

        }
        public async Task<object> Get(GetGpsInfoByIdReq req)
        {
            return Ok(await _mediator.Send(new GetGpsInfoByIdQuery(req.Id)));
        }
        public async Task<object> Get(GetTotalGprsReq req)
        {
            return await _mediator.Send(new GetTotalGprsQuery());
        }

        #endregion

        #region POST methods
        public async Task<object> Post(CreateGpsInfoReq req)
        {
            var formData = Request.FormData;
            if (formData != null)
            {
                req.GpsInfo = formData["gpsInfo"].FromJson<GpsInfoDto>() ?? new GpsInfoDto();
                
            }
            var result = await _mediator.Send(new CreateGpsInfoCommand(req.GpsInfo));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }
            return Ok(result);

        }
        #endregion

        #region Put method
        public async Task<object> Put(UpdateGpsInfoReq req)
        {
            var formData = Request.FormData;
            if (formData != null)
            {
                req.GpsInfo = formData["gpsInfo"].FromJson<GpsInfoDto>();               
            }

            var result = await _mediator.Send(new UpdateGpsInfoCommand(req.GpsInfo));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(result);
        }
        public async Task<object> Put(ChangeQrKeyGpsInfoReq req)
        {
            var result = await _mediator.Send(new UpdateQrkeyCommand(req.Id));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(result);
        }
        #endregion

        #region Delete methods
        public async Task<object> Delete(DeleteGpsInfoReq req)
        {
            await _mediator.Send(new DeleteGpsInfoCommand(req.Id));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed,"GpsInfo"));
        }
        #endregion
    }
}
