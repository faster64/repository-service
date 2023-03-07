using KiotVietTimeSheet.Api.ServiceModel;
using System.Threading.Tasks;
using ServiceStack;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Application.Abstractions;
using MediatR;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Commands.CancelPayslip;
using KiotVietTimeSheet.Application.Commands.ExportEmployeePayslipData;
using KiotVietTimeSheet.Application.Commands.ExportPayslipData;
using KiotVietTimeSheet.Application.Queries.GetExportPayslipData;
using KiotVietTimeSheet.Application.Queries.GetPayslipById;
using KiotVietTimeSheet.Application.Queries.GetPayslipsByFilter;
using KiotVietTimeSheet.Application.Queries.GetPayslipsByPaysheetId;
using KiotVietTimeSheet.Application.Queries.GetPaySlipsByQueryFilter;
using KiotVietTimeSheet.Application.Queries.GetPayslipsClockingByPayslipId;
using KiotVietTimeSheet.Application.Queries.GetUnPaidPayslipByEmployeeId;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class PayslipApi : BaseApi
    {
        private readonly IMediator _mediator;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        public IAutoQueryDb AutoQuery { get; set; }

        public PayslipApi(
            ILogger<PayslipApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<object> Get(GetPayslipsByFilterReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetPayslipsByFilterQuery(query));
            return Ok(result);
        }

        public async Task<object> Get(GetPayslipsByPaysheetIdReq req)
        {
            var filter = new PayslipByPaysheetIdFilter
            {
                PaysheetId = req.PaysheetId,
                Skip = req.Skip,
                Take = req.Take,
                OrderBy = req.OrderBy != null && req.OrderBy.Any() ? req.OrderBy : new[] { "CreatedDate" },
                OrderByDesc = req.OrderByDesc,
                PayslipStatuses = req.PayslipStatuses
            };
            var result = await _mediator.Send(new GetPayslipsByPaysheetIdQuery(filter));

            return Ok(result);
        }

        public async Task<object> Get(GetPayslipsClockingByPayslipIdReq req)
        {
            var filter = new PayslipClockingByPayslipIdFilter
            {
                PayslipId = req.PayslipId,
                Skip = req.Skip,
                Take = req.Take,
                OrderBy = req.OrderBy != null && req.OrderBy.Any() ? req.OrderBy : new[] { "CheckInDate" },
                OrderByDesc = req.OrderByDesc
            };
            var result = await _mediator.Send(new GetPayslipsClockingByPayslipIdQuery(filter));
            return Ok(result);
        }

        public async Task<object> Get(GetExportPayslipDataReq req)
        {
            var result = await _mediator.Send(new GetExportPayslipDataQuery(req.Filters.ConvertTo<PayslipByPaysheetIdFilter>()));
            return Ok(new
            {
                Payslips = result
            }
            );
        }

        public async Task<object> Get(GetPaySlipsByQueryFilter req)
        {
            var filters = new PayslipByPaysheetIdFilter
            {
                PayslipStatuses = req.PayslipStatuses?.ToList() ?? new List<byte>(),
                EmployeeId = req.EmployeeId,
                PaysheetId = req.PaysheetId,
                PayslipIds = req.PayslipIds,
                OrderByDesc = req.OrderByDesc
            };

            var result = await _mediator.Send(new GetPaySlipsByQueryFilterQuery(filters));
            return Ok(result);
        }
        public async Task<object> Get(GetPaySlipById req)
        {
            var result = await _mediator.Send(new GetPayslipByIdQuery(req.Id));
            return Ok(result);
        }

        public async Task<object> Get(GetUnPaidPayslipByEmployeeIdReq req)
        {
            var unpaidPayslipsAndPaysheet = await _mediator.Send(new GetUnPaidPayslipByEmployeeIdQuery(req.EmployeeId));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenGetData, Errors);
            }

            return Ok(unpaidPayslipsAndPaysheet);
        }

        public async Task<object> Post(PostExportPayslipDataReq req)
        {
            var result =
                    await _mediator.Send(
                        new ExportPayslipDataCommand(req.Filters.ConvertTo<PayslipByPaysheetIdFilter>()));
            return Ok(new
            {
                Payslips = result
            }
            );
        }

        public async Task<object> Post(PostExportEmployeePayslipDataReq req)
        {
            var result = await _mediator.Send(
                    new ExportEmployeePayslipCommand(req.PaysheetId, req.BranchId, req.EmployeeId, req.PayslipId));

            return Ok(new
            {
                EmployeePayslipDto = result
            }
            );
        }
        #region Internal Request
        public object Post(CreatedPayslipPaymentEventReq req){
            _timeSheetIntegrationEventService.PublishEvent(req.Event);
            return Ok("Publish Message Successfully");
        }

        public object Post(VoidPayslipPaymentEventReq req)
        {
            _timeSheetIntegrationEventService.PublishEvent(req.Event);
            return Ok("Publish Message Successfully");
        }
        #endregion

        public async Task<object> Put(CancelPayslipReq req)
        {
            var result = await _mediator.Send(new CancelPayslipCommand(req.Id, req.IsCheckPayslipPayment, req.IsCancelPayment));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(result);
        }

                    
    }
}
