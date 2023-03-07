using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Commands.UpdateConfirmClocking;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetConfirmClockingsByBranchId;
using KiotVietTimeSheet.Application.Queries.GetListConfirmClocking;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;
using static KiotVietTimeSheet.Domain.Utilities.Utilities;
namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class ConfirmClockingApi : BaseApi
    {
        #region Properties
        private readonly IMediator _mediator;
        public IAutoQueryDb AutoQuery { get; set; }
        #endregion
        public ConfirmClockingApi(
           ILogger<ConfirmClockingApi> logger,
           INotificationHandler<DomainNotification> notificationHandler,
           IMediator mediator
            ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Get(GetListConfirmClockingReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());

            if (req.EmployeeNames != null && req.EmployeeNames.Length > 0)
            {
                query.Join<Employee>((x, y) => x.EmployeeId == y.Id);
                query.Where(x => req.EmployeeNames.Contains(x.Employee.Name));
            }
            if (req.BranchIds != null && req.BranchIds.Length > 0)
            {
                query.Join<GpsInfo>((x, y) => x.GpsInfoId == y.Id);
                query.Where(x => req.BranchIds.Contains(x.GpsInfo.BranchId));
            }
            if (req.TimeRange != null && req.TimeRange != "alltime")
            {
                var dateTimeFilter = new DateTimeFilter() { StartDate = req.StartDate, EndDate = req.EndDate, TimeRange = req.TimeRange };
                dateTimeFilter = GetTimeRangeBase(dateTimeFilter, true);
                if (dateTimeFilter.StartDate != null)
                {
                    query.Where(x => x.CreatedDate >= dateTimeFilter.StartDate);
                }
                if (dateTimeFilter.EndDate != null)
                {
                    query.Where(x => x.CreatedDate <= dateTimeFilter.EndDate);
                }
            }
            query.Where(x => x.Status == 0);
            query.OrderByDescending(x => x.CreatedDate);
            var result = await _mediator.Send(new GetListConfirmClockingQuery(query, req.WithDeleted));
            return Ok(result);

        }
        public async Task<object> Get(GetConfirmClockingsByBranchIdReq req)
        {
            var result = await _mediator.Send(new GetConfirmClockingsByBranchIdQuery(req.BranchId));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(result);
        }
        public async Task<object> Put(UpdateConfirmClockingReq req)
        {
            var formData = Request.FormData;
            if (formData != null)
            {
                req.lsConfirmClocking = formData["confirmClockings"].FromJson<List<ConfirmClockingDto>>();
            }

            var result = await _mediator.Send(new UpdateConfirmClockingCommand(req.lsConfirmClocking));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(result);
        }
    }
}
