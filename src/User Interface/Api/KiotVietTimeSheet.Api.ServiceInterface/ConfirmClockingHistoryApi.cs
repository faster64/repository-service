using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Queries.GetListConfirmClockingHistory;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;
using static KiotVietTimeSheet.Domain.Utilities.Utilities;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class ConfirmClockingHistoryApi : BaseApi
    {
        #region Properties
        private readonly IMediator _mediator;
        public IAutoQueryDb AutoQuery { get; set; }
        #endregion
        public ConfirmClockingHistoryApi(
           ILogger<ConfirmClockingHistoryApi> logger,
           INotificationHandler<DomainNotification> notificationHandler,
           IMediator mediator
            ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Get(GetListConfirmClockingHistoryReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            query.Join<ConfirmClocking>((x, y) => x.ConfirmClockingId == y.Id);
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
            if (req.EmployeeNames != null && req.EmployeeNames.Length > 0)
            {
                query.Join<Employee>((x, y) => x.ConfirmClocking.EmployeeId == y.Id);
                query.Where(x => req.EmployeeNames.Contains(x.ConfirmClocking.Employee.Name));
            }
            if (req.BranchIds != null && req.BranchIds.Length > 0)
            {
                query.Join<GpsInfo>((x, y) => x.ConfirmClocking.GpsInfoId == y.Id);
                query.Where(x => req.BranchIds.Contains(x.ConfirmClocking.GpsInfo.BranchId));
            }
            query.OrderByDescending(x => x.CreatedDate);
            var result = await _mediator.Send(new GetListConfirmClockingHistoryQuery(query, req.WithDeleted));
            return Ok(result);

        }
    }
}
