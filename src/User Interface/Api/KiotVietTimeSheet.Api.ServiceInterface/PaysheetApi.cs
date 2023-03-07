using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Commands.AutoLoadingAndUpdatePaySheet;
using KiotVietTimeSheet.Application.Commands.CancelPaysheet;
using KiotVietTimeSheet.Application.Commands.ChangeVersionPaysheet;
using KiotVietTimeSheet.Application.Commands.CompletePaysheet;
using KiotVietTimeSheet.Application.Commands.CreatePaysheet;
using KiotVietTimeSheet.Application.Commands.ExportPaySheet;
using KiotVietTimeSheet.Application.Commands.ExportPaySheetCommissionDetailData;
using KiotVietTimeSheet.Application.Commands.ExportPaySheetData;
using KiotVietTimeSheet.Application.Commands.PaysheetWhenChangeWorkingPeriod;
using KiotVietTimeSheet.Application.Commands.UpdatePaysheet;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Queries.GetDraftPaysheet;
using KiotVietTimeSheet.Application.Queries.GetGenerateWorkingPeriod;
using KiotVietTimeSheet.Application.Queries.GetPaysheetAndCheckChangeById;
using KiotVietTimeSheet.Application.Queries.GetPaysheetAndCheckChangeVersion;
using KiotVietTimeSheet.Application.Queries.GetPaysheetById;
using KiotVietTimeSheet.Application.Queries.GetPaysheetExistsPayslips;
using KiotVietTimeSheet.Application.Queries.GetPaysheets;
using KiotVietTimeSheet.Application.Queries.GetPaysheetsOldVersionByIds;
using KiotVietTimeSheet.Application.Queries.GetPaysheetsWorkingDays;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;
using Message = KiotVietTimeSheet.Resources.Message;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class PaysheetApi : BaseApi
    {
        private readonly IMediator _mediator;

        public PaysheetApi(
            ILogger<PaysheetApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Get(GetPaysheets req)
        {
            var result = await _mediator.Send(new GetPaysheetsQuery(new PaySheetQueryFilter
            {
                PaysheetKeyword = req.PaysheetKeyword,
                EmployeeKeyword = req.EmployeeKeyword,
                BranchIds = req.BranchIds,
                PaysheetStatuses = req.PaysheetStatuses,
                SalaryPeriod = req.SalaryPeriod,
                StartTime = req.StartTime,
                EndTime = req.EndTime,
                Skip = req.Skip,
                Take = req.Take,
                OrderByDesc = req.OrderByDesc,
                OrderBy = req.OrderBy
            }));

            return Ok(result);
        }

        public async Task<object> Get(GetCheckExistsPayslip req)
        {
            var result = await _mediator.Send(new GetPaysheetExistsPayslipsQuery(req.PaysheetId));
            return Ok(result, "");
        }

        public async Task<object> Get(GetWorkingDayNumber req)
        {
            var workingDayNumber = await _mediator.Send(new GetPaysheetWorkingDaysQuery(req.StartTime, req.EndTime));
            return Ok(new { workingDayNumber });
        }

        public async Task<object> Get(GetPaysheetById req)
        {
            return Ok(await _mediator.Send(new GetPaysheetByIdQuery(req.Id)));
        }

        public async Task<object> Get(GetAndCheckChangeById req)
        {
            return Ok(await _mediator.Send(new GetPaysheetAndCheckChangeByIdQuery(req.Id, req.BranchId, req.KvSessionBranchId)));
        }

        public async Task<object> Get(CheckChangeVersionPaysheetReq req)
        {
            var result = await _mediator.Send(new GetPaysheetAndCheckChangeVersionQuery(req.Id, req.Version));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(result, "");
        }

        public async Task<object> Get(GetPaysheetsOldVersionByIdsReq req)
        {
            var result = await _mediator.Send(new GetPaysheetsOldVersionByIdsQuery(req.Ids));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return result;
        }

        public async Task<object> Get(GetDraftPaysheet req)
        {
            var returnObj = await _mediator.Send(new GetDraftPaysheetQuery(req.SalaryPeriod, req.StartTime, req.EndTime, req.EmployeeIds, req.BranchId, req.PaysheetId, req.WorkingDayNumber));
            return Ok(returnObj);
        }

        public async Task<object> Get(GenerateWorkingPeriod req)
        {
            var returnObj = await _mediator.Send(new GenerateWorkingPeriodQuery(req.SalaryPeriodType, req.StartDate, req.EndDate, req.IsUpdatePaysheet));
            return Ok(returnObj);
        }

        public async Task<object> Get(GetExportPaySheetDataReq req)
        {
            var result = await _mediator.Send(new ExportPaySheetCommand((req.Filters.ConvertTo<PaySheetQueryFilter>())));
            return Ok(new
            {
                Paysheets = result.Data
            });
        }

        #region POST Methods
        public async Task<object> Post(PostExportPaySheetDataReq req)
        {
            var result = await _mediator.Send(new ExportPaySheetDataCommand(req.Filters.ConvertTo<PaySheetQueryFilter>()));
            return Ok(new
            {
                Paysheets = result.Data
            });
        }
        public async Task<object> Post(GetPaysheetWhenChangeWorkingPeriod req)
        {
            if (req.PaysheetDto.StartTime.AddDays(Domain.Common.Constant.PaysheetPeriodOptionDays) < req.PaysheetDto.EndTime)
            {
                return BadRequest(string.Format(Message.timeSheet_createPaysheetPeriodOptionDays, Domain.Common.Constant.PaysheetPeriodOptionDays), Errors);
            }
            var result = await _mediator.Send(new PaysheetWhenChangeWorkingPeriodCommand(req.PaysheetDto, req.Branches));
            return Ok(result);
        }

        public async Task<object> Post(CompletePaysheet req)
        {
            var returnObj = await _mediator.Send(new CompletePaysheetCommand(req.Paysheet, req.IsCheckPayslipPayment, req.IsCancelPayment));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Post(ChangeVersionPaysheetReq req)
        {
            await _mediator.Send(new ChangeVersionPaysheetCommand(req.Ids));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(Message.payslip_updateVersionSuccessfully);
        }
        public async Task<object> Post(CreatePaysheet req)
        {
            if(req.StartTime.AddDays(Domain.Common.Constant.PaysheetPeriodOptionDays) < req.EndTime)
            {
                return BadRequest(string.Format(Message.timeSheet_createPaysheetPeriodOptionDays, Domain.Common.Constant.PaysheetPeriodOptionDays), Errors);
            }

            var result = await _mediator.Send(new CreatePaysheetCommand(req.SalaryPeriod, req.StartTime, req.EndTime, req.Branches));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(result);
        }
        public async Task<object> Post(PostExportPaySheetCommissionDetailDataReq req)
        {
            var result = await _mediator.Send(
                new ExportPaySheetCommissionDetailCommand(req.Id, req.BranchId, req.EmployeeId, req.PayslipId));

            return Ok(new
            {
                PaysheetCommissionDetail = result
            });
        }
        #endregion

        #region PUT Methods
        public async Task<object> Put(CancelPaysheetReq req)
        {
            var result =
                    await _mediator.Send(new CancelPaysheetCommand(req.Id, req.IsCheckPayslipPayment,
                        req.IsCancelPayment));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(result);
        }

        public async Task<object> Put(UpdatePaysheet req)
        {
            var returnObj = await _mediator.Send(new UpdatePaysheetCommand(req.Id, req.Paysheet,
                req.IsCheckPayslipPayment, req.IsCancelPayment));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Put(AutoLoadingAndUpdateDataPaysheetReq req)
        {
            var returnObj = await _mediator.Send(new AutoLoadingAndUpdatePaysheetCommand(req.Id, req.ModifiedDate, req.IsAcceptLoading, req.Branches));
            
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }
        #endregion

    }
}
