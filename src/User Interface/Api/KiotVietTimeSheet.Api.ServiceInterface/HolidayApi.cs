using KiotVietTimeSheet.Api.ServiceModel;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;
using ServiceStack.OrmLite;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Commands.CreateHoliday;
using KiotVietTimeSheet.Application.Commands.CreateNationalHoliday;
using KiotVietTimeSheet.Application.Commands.DeleteHoliday;
using KiotVietTimeSheet.Application.Commands.UpdateHoliday;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetHoliday;
using KiotVietTimeSheet.Application.Queries.GetHolidayById;
using KiotVietTimeSheet.Application.Queries.GetHolidayTotalDays;
using Message = KiotVietTimeSheet.Resources.Message;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class HolidayApi : BaseApi
    {
        #region Properties
        public IAutoQueryDb AutoQuery { get; set; }
        private readonly IMediator _mediator;
        #endregion

        #region Constructor
        public HolidayApi(
            ILogger<HolidayApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }
        #endregion

        #region GET methods 
        public async Task<object> Get(GetListHolidayReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetHolidayQuery(query));

            if (result!=null && result.Data != null && result.Data.Count > 0 && req.HasSummaryRow)
            {
                query.Select(x => Sql.Sum(x.Days));
                query.Where(x => !x.IsDeleted);
                query.Skip(null);
                query.Take(null);
                query.OrderBy();
                result.TotalDays = await _mediator.Send(new GetHolidayTotalDaysQuery(query));
                var sumRow = new HolidayDto
                {
                    Days = result.TotalDays,
                    Id = -1
                };

                result.Data.Insert(0, sumRow);
            }

            return Ok(result);
        }

        public async Task<object> Get(GetHolidayByIdReq req)
        {
            return Ok(await _mediator.Send(new GetHolidayByIdQuery(req.Id)));
        }
        #endregion

        #region POST methods
        public async Task<object> Post(CreateHolidayReq req)
        {
            var returnObj =
                    await _mediator.Send(new CreateHolidayCommand(req.Holiday, req.IsCancelClocking,
                        req.IsOverLapClocking));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Post(CreateNationalHolidayReq req)
        {

            await _mediator.Send(new CreateNationalHolidayCommand());
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }
            return Ok(string.Format(Message.create_successed, Label.holiday_t.ToLower()));
        }
        #endregion

        #region PUT methods
        public async Task<object> Put(UpdateHolidayReq req)
        {
            var returnObj = await _mediator.Send(new UpdateHolidayCommand(req.Holiday, req.IsAddClocking.GetValueOrDefault(), req.IsCancelClocking.GetValueOrDefault(), req.IsOverLapClocking));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }
        #endregion

        #region DELETE methods
        public async Task<object> Delete(DeleteHolidayReq req)
        {
            var returnObj = await _mediator.Send(new DeleteHolidayCommand(req.Id, req.IsAddClocking.GetValueOrDefault(), req.IsOverLapClocking));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(returnObj);
        }
        #endregion
    }
}
