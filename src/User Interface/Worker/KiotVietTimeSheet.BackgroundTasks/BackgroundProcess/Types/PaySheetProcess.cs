
using System;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.Application.Queries.GetPaysheetById;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByBranchId;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using KiotVietTimeSheet.Application.Commands.CreatePaysheetPayslip;
using KiotVietTimeSheet.Application.Commands.UpdatePaysheetFailed;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetAllDeductionByTenantD;
using KiotVietTimeSheet.Application.Queries.GetAllowanceByTenantId;
using KiotVietTimeSheet.Application.Queries.GetClockingForPaySheet;
using KiotVietTimeSheet.Application.Queries.GetEmployeeBranchByEmployeeIds;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Application.Queries.GetCommissionByIds;
using KiotVietTimeSheet.Application.Queries.GetHolidayForPaySheet;
using KiotVietTimeSheet.Application.Queries.GetPayRateByEmployeeIds;
using KiotVietTimeSheet.Application.Queries.GetShiftByBranchId;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Common;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using Newtonsoft.Json;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.BackgroundTasks.EventHandlers;
using KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.EventBus;
using KiotVietTimeSheet.Application.Commands.UpdatePaySheetPayslips;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.BackgroundTasks.Infras.Mqtt;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using Microsoft.Extensions.Configuration;
using KiotVietTimeSheet.Application.Commands.UpdatePaysheetTemporaryStatus;
using KiotVietTimeSheet.Application.Queries.GetClockingPenalizeForPaySheet;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Infrastructure.AuditLog;
using KiotVietTimeSheet.Application.Queries.GetPaysheetByIdForBgTask;
using ServiceStack;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types
{
    public class PaySheetProcess : BaseBackgroundProcess
    {
        private readonly ILogger<PaySheetIntegrationEventHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly AuditLogProcess _auditLogProcess;
        private readonly MqttClientWrapper _mqttClient;
        private readonly bool _useMqtt;
        private readonly bool _turnErrorPaysheetOn;
        private int _descriptionError = 0;

        public PaySheetProcess(
            IMediator mediator,
            ILogger<PaySheetIntegrationEventHandler> logger,
            IKiotVietInternalService kiotVietInternalService,
            IMapper mapper,
            AuditLogProcess auditLogProcess,
            IAuthService authService,
            IConfiguration configuration,
            MqttClientWrapper mqttClient) : base(kiotVietInternalService,
            authService)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _auditLogProcess = auditLogProcess;
            _mqttClient = mqttClient;
            _useMqtt = bool.Parse(configuration.GetValue<string>("UseMqtt"));
            _turnErrorPaysheetOn = bool.Parse(configuration.GetValue<string>("TurnOnPaysheetError"));
        }

        public async Task CreatePaySheet(CreatePaysheetEmptyIntegrationEvent @event)
        {
            try
            {
                var paySheetDto = await _mediator.Send(new GetPaysheetByIdQuery(@event.PaySheetId, false));
                if (paySheetDto.PaysheetStatus != (byte)PaysheetStatuses.Pending)
                {
                    if (_useMqtt)
                    {
                        await PublishMessageAsync(paySheetDto, EventTypeStatic.PaySheetEmptyPendingIntegrationSocket,
                            "", paySheetDto.PaysheetStatus, (int)HttpStatusCode.OK, @event.Context);
                    }
                    return;
                }

                if (paySheetDto.PaysheetStatus == (byte)PaysheetStatuses.Pending && _useMqtt)
                {
                    await PublishMessageAsync(paySheetDto, EventTypeStatic.PaySheetEmptyPendingIntegrationSocket,
                        "", (byte)PaysheetStatuses.Pending, (int)HttpStatusCode.OK, @event.Context);
                }

                var (employees, listEmployeeId) = await GetListEmployeesDto(paySheetDto.BranchId, paySheetDto, @event.Context, false);

                var listPayRatedDto = await GetListPayrateProcessAsync(listEmployeeId, paySheetDto, @event.Context, false);

                var setting = await _mediator.Send(new GetSettingQuery(paySheetDto.TenantId));

                var (userRevenues, userProductRevenues) = await GetApiInternalRevenue(employees, paySheetDto, @event.Context, false, setting: setting);

                var (userCounselorRevenues, userCounselorProductRevenues) = await GetApiInternalRevenueCounselor(employees, paySheetDto, @event.Context, false, setting: setting);

                var (listClocking, listPaidClocking, listUnPaidClocking, listShiftDto, listHolidayDto, listClockingPenalizeDto) = await GetClockingProcessAsync(paySheetDto.StartTime, paySheetDto.EndTime, listEmployeeId, paySheetDto.BranchId, paySheetDto, @event.Context, false);

                var (productBranchRevenues, listCommissionDto) = await GetCommissionProcessAsync(listPayRatedDto, paySheetDto, @event.BranchesDto, @event.Context, false);

                var deductionsTask = GetDeductionProcessAsync(paySheetDto.TenantId, paySheetDto, @event.Context, false);

                var allowancesTask = GetAllowanceProcessAsync(paySheetDto.TenantId, paySheetDto, @event.Context, false);

                await Task.WhenAll(deductionsTask, allowancesTask);

                var settingObjDto = await _mediator.Send(new GetSettingQuery(paySheetDto.TenantId));

                var resultPaySheetDto = await CreatePaySheetPayslips(
                    paySheetDto, employees, userRevenues, userCounselorRevenues, deductionsTask.Result, allowancesTask.Result, listCommissionDto,
                    listClocking, listPaidClocking, listUnPaidClocking,
                    listPayRatedDto, listHolidayDto, listShiftDto, userProductRevenues, userCounselorProductRevenues, productBranchRevenues,
                    listClockingPenalizeDto, @event.Context, false, settingObjDto);

                var eventContext = GetIntegrationEventContext(paySheetDto, @event.Context);

                var eventAuditPaySheet = new CreatePaysheetProcessIntegrationEvent(paySheetDto.Id, paySheetDto.Code, paySheetDto.Name, paySheetDto.PaysheetStatus);
                eventAuditPaySheet.SetContext(eventContext);

                _auditLogProcess.SendEventData(eventAuditPaySheet, nameof(CreatePaysheetProcessIntegrationEvent));

                var descriptions = string.Format(Message.create_successed, Label.paysheet.ToLower());
                if (_useMqtt)
                {
                    await PublishMessageAsync(paySheetDto, EventTypeStatic.CreateDraftPaySheetIntegrationSocket,
                    descriptions, resultPaySheetDto.PaysheetStatus, (int)HttpStatusCode.OK, @event.Context);
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("UpdatePaysheetFailedCommand with Event" + @event.ToSafeJson());
                _logger.LogError(ex, ex.Message);
                await _mediator.Send(new UpdatePaysheetFailedCommand(@event.PaySheetId));
                var paySheetDto = await _mediator.Send(new GetPaysheetByIdForBgTaskQuery(@event.PaySheetId, false));
                if (_useMqtt)
                {
                    await PublishMessageAsync(paySheetDto, EventTypeStatic.CreateDraftPaySheetIntegrationSocket,
                    _descriptionError.ToString(), (byte)PaysheetStatuses.Void, (int)HttpStatusCode.InternalServerError, @event.Context);
                }

            }
        }

        private async Task PublishMessageAsync(PaysheetDto paySheetDto, string eventType, string descriptions, byte paySheetStatus, int statusCode, IntegrationEventContext context)
        {
            var log = new LogObject(_logger, Guid.NewGuid())
            {
                Action = "MqttPublishMessage",
                RetailerId = context.TenantId,
                RetailerCode = context.RetailerCode,
                BranchId = context.BranchId
            };

            try
            {
                if (paySheetDto.TenantId < 1 || paySheetDto.BranchId < 1)
                {
                    return;
                }

                var eventContext = GetIntegrationEventContext(paySheetDto, context);

                var eventPaySheet = new SocketPaySheetCreateIntegrationEvent(
                    context.TenantId, context.BranchId, paySheetDto.Id,
                    paySheetStatus, descriptions,
                    eventType, statusCode);
                eventPaySheet.SetContext(eventContext);

                var mqttMsg = new MqttClientPublishMessage
                {
                    Topic = $"timesheet/{context.TenantId}/{context.BranchId}",
                    Message = JsonConvert.SerializeObject(eventPaySheet,
                        new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                    Qos = 0,
                    Retain = false
                };

                log.RequestObject = mqttMsg;

                await _mqttClient.PublishAsync(mqttMsg);

                log.Description = "Publish mqtt message successful";
                log.LogInfo();
            }
            catch (Exception ex)
            {
                log.Description = "Publish mqtt message failed";
                log.LogError(ex);
            }
        }

        private IntegrationEventContext GetIntegrationEventContext(PaysheetDto paySheetDto, IntegrationEventContext context)
        {
            return new IntegrationEventContext(
                context.TenantId,
                context.BranchId,
                paySheetDto.CreatedBy,
                _authService.Context.User.GroupId,
                _authService.Context.TenantCode,
                context.User,
                _authService.Context.Language
            );
        }

        public async Task ChangePeriodPaySheet(ChangePeriodPaysheetIntegrationEvent @event)
        {
            var eventAutoLoadPaysheet =
                new AutoLoadingAndUpdatePaysheetIntegrationEvent(@event.PaySheetId, null, null, @event.BranchesDto, @event.PaySheetCreateTimeOld);
            eventAutoLoadPaysheet.SetContext(@event.Context);
            await AutoLoadingAndUpdatePaySheet(eventAutoLoadPaysheet, @event.PaysheetOldDto);
        }

        /// <summary>
        /// Nếu trường hợp bảng lương cũ khác null => Người dùng chuyển kỳ trong cập nhật bảng lương
        /// </summary>
        /// <param name="event"></param>
        /// <param name="paysheetOldDto">Chỉ có trường hợp chuyển kỳ trong cập nhật mới có dữ liệu</param>
        /// <returns></returns>
        public async Task AutoLoadingAndUpdatePaySheet(AutoLoadingAndUpdatePaysheetIntegrationEvent @event, PaysheetDto paysheetOldDto = null)
        {
            try
            {
                var paySheetDto = await _mediator.Send(new GetPaysheetByIdQuery(@event.PaySheetId, false));

                if (paySheetDto.PaysheetStatus != (byte)PaysheetStatuses.Pending)
                {
                    return;
                }

                if (paySheetDto.PaysheetStatus == (byte)PaysheetStatuses.Pending && _useMqtt)
                {
                    await PublishMessageAsync(paySheetDto, EventTypeStatic.PaySheetPendingIntegrationSocket,
                        "", (byte)PaysheetStatuses.Pending, (int)HttpStatusCode.OK, @event.Context);
                }

                var (employees, listEmployeeId) = await GetListEmployeesDto(paySheetDto.BranchId, paySheetDto, @event.Context, true, @event.PaySheetCreateTimeOld, paysheetOldDto);
                if (@event.TimeOfStandardWorkingDay != null)
                {
                    paySheetDto.TimeOfStandardWorkingDay = @event.TimeOfStandardWorkingDay.Value;
                }

                if (@event.StandardWorkingDayNumber != null)
                {
                    paySheetDto.WorkingDayNumber = @event.StandardWorkingDayNumber.Value;
                }


                var listPayRatedDto = await GetListPayrateProcessAsync(listEmployeeId, paySheetDto, @event.Context, true, @event.PaySheetCreateTimeOld, paysheetOldDto);

                var setting = await _mediator.Send(new GetSettingQuery(paySheetDto.TenantId));

                var (userRevenues, userProductRevenues) = await GetApiInternalRevenue(employees, paySheetDto, @event.Context, true, @event.PaySheetCreateTimeOld, paysheetOldDto, setting: setting);

                var (listClocking, listPaidClocking, listUnPaidClocking, listShiftDto, listHolidayDto, listClockingPenalizeDto) = await GetClockingProcessAsync(paySheetDto.StartTime, paySheetDto.EndTime, listEmployeeId, paySheetDto.BranchId, paySheetDto, @event.Context, true, @event.PaySheetCreateTimeOld, paysheetOldDto);

                var (productBranchRevenues, listCommissionDto) = await GetCommissionProcessAsync(listPayRatedDto, paySheetDto, @event.BranchesDto, @event.Context, true, @event.PaySheetCreateTimeOld, paysheetOldDto);

                var (userCounselorRevenues, userCounselorProductRevenues) = await GetApiInternalRevenueCounselor(employees, paySheetDto, @event.Context, false, setting: setting);

                var deductionsTask = GetDeductionProcessAsync(paySheetDto.TenantId, paySheetDto, @event.Context, true, @event.PaySheetCreateTimeOld, paysheetOldDto);

                var allowancesTask = GetAllowanceProcessAsync(paySheetDto.TenantId, paySheetDto, @event.Context, true, @event.PaySheetCreateTimeOld, paysheetOldDto);

                await Task.WhenAll(deductionsTask, allowancesTask);

                var settingObjDto = await _mediator.Send(new GetSettingQuery(paySheetDto.TenantId));

                await UpdatePaySheetPayslips(
                    paySheetDto, employees, userRevenues, userCounselorRevenues, deductionsTask.Result, allowancesTask.Result, listCommissionDto,
                    listClocking, listPaidClocking, listUnPaidClocking,
                    listPayRatedDto, listHolidayDto, listShiftDto, userProductRevenues, userCounselorProductRevenues, productBranchRevenues, listClockingPenalizeDto, @event.Context, true, settingObjDto);
            }
            catch
            {
                if (paysheetOldDto != null)
                {
                    paysheetOldDto.ErrorStatus = _descriptionError;
                }

                await _mediator.Send(new UpdatePaysheetTemporaryStatusCommand(@event.PaySheetId, @event.PaySheetCreateTimeOld, paysheetOldDto, _descriptionError));
                var paySheetDto = await _mediator.Send(new GetPaysheetByIdQuery(@event.PaySheetId, false));
                if (_useMqtt)
                {
                    await PublishMessageAsync(paySheetDto, EventTypeStatic.AutoLoadingAndUpdatePaySheetIntegrationSocket,
                    _descriptionError.ToString(), (byte)PaysheetStatuses.Void, (int)HttpStatusCode.InternalServerError, @event.Context);
                }
            }
        }

        #region process COMMISSION
        private CommissionSalaryRuleValueV2 GetCommissionRuleValue(List<PayRateDetail> payRateDetails)
        {

            var commissionRule = payRateDetails.FirstOrDefault(p => p.RuleType == typeof(CommissionSalaryRuleV2).Name);
            var commissionSalaryValue = new CommissionSalaryRuleValueV2();
            if (commissionRule != null)
            {
                commissionSalaryValue = (CommissionSalaryRuleValueV2)JsonConvert.DeserializeObject(commissionRule.RuleValue,
                    typeof(CommissionSalaryRuleValueV2));
            }
            return commissionSalaryValue;

        }

        /// <summary>
        /// Lấy doanh thu theo chi nhánh và list commission in source.
        /// </summary>
        /// <param name="listPayRatedDto"></param>
        /// <param name="paysheetDto"></param>
        /// <param name="branchedDto"></param>
        /// <param name="context"></param>
        /// <param name="isUpdate"></param>
        /// <param name="payheetCreateOldTime"></param>
        /// <param name="paysheetOldDto">Chỉ có trường hợp chuyển kỳ trong cập nhật mới có dữ liệu</param>
        /// <returns> 
        /// Return two items.
        /// Item 1: List ProductRevenue (Doanh thu chi nhánh)
        /// Item 2: List Commission in source
        /// </returns>
        private async Task<Tuple<List<ProductRevenue>, List<CommissionDto>>> GetCommissionProcessAsync(
            List<PayRate> listPayRatedDto, PaysheetDto paysheetDto, List<BranchDto> branchedDto,
            IntegrationEventContext context, bool isUpdate, DateTime? payheetCreateOldTime = null, PaysheetDto paysheetOldDto = null)
        {
            try
            {
                //check paysheet case failed
                if (_turnErrorPaysheetOn)
                    throw new Exception();//NOSONAR

                //start Kiểm tra nếu có bất kỳ nhân viên nào có cài đặt tính hoa hồng cho tất cả chi nhánh
                var payRateDetails = listPayRatedDto.Select(p => p.PayRateDetails).ToList();

                var branchIds = new List<long>();
                var commissionIds = new List<long>();
                var listCommissionSalaryRuleValue = payRateDetails.Select(GetCommissionRuleValue).ToList();
                listCommissionSalaryRuleValue = listCommissionSalaryRuleValue.Where(c => c.CommissionSalaryRuleValueDetails != null).ToList();
                foreach (var commissionSalaryValue in listCommissionSalaryRuleValue)
                {
                    //Lấy danh sách các bảng hoa hồng được gán cho nhân viên
                    commissionIds.AddRange(commissionSalaryValue.CommissionSalaryRuleValueDetails
                        .Select(c => c.CommissionTableId ?? 0).ToList());

                    //Nếu có bất kỳ nhân viên nào có thiết lập hoa hồng chi nhánh mà chọn All chi nhánh
                    if (commissionSalaryValue.FormalityTypes !=
                        CommissionSalaryFormalityTypes.BranchCommissionRevenue) continue;

                    if (commissionSalaryValue.BranchIds != null && commissionSalaryValue.BranchIds.Count > 0)
                        branchIds.AddRange(commissionSalaryValue.BranchIds.Select(x => (long)x).ToList());
                }

                commissionIds = commissionIds.Where(c => c != 0).Distinct().ToList();

                var branchProductRevenuesTask = GetProductBranchRevenue(paysheetDto, branchedDto, branchIds, listCommissionSalaryRuleValue);
                var commissionTablesTask = GetListCommissionByCommissionIds(commissionIds);

                await Task.WhenAll(branchProductRevenuesTask, commissionTablesTask);

                return new Tuple<List<ProductRevenue>, List<CommissionDto>>(branchProductRevenuesTask.Result, commissionTablesTask.Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                var eventContext = GetIntegrationEventContext(paysheetDto, context);
                _descriptionError = (int)PaysheetErrorCode.Commision;
                var paySheetStatus = isUpdate ? (byte)PaysheetStatuses.TemporarySalary : paysheetDto.PaysheetStatus;
                if (payheetCreateOldTime != null)
                {
                    paysheetDto.PaysheetCreatedDate = payheetCreateOldTime;
                }
                if (paysheetOldDto != null)
                {
                    UpdateOldPaysheetDto(paysheetDto, paysheetOldDto);
                }
                var eventAuditLogProcessError = new UpdatePaysheetProcessErrorIntegrationEvent(paysheetDto.Code, paysheetDto.Name,
                    paySheetStatus, "Lỗi khi lấy thông tin bảng hoa hồng", paysheetDto, isUpdate);
                eventAuditLogProcessError.SetContext(eventContext);
                _auditLogProcess.SendEventData(eventAuditLogProcessError, nameof(UpdatePaysheetProcessErrorIntegrationEvent));
                throw;
            }
        }

        private async Task<List<CommissionDto>> GetListCommissionByCommissionIds(List<long> commissionIds)
        {
            var commissionTables = await _mediator.Send(new GetCommissionByIdsQuery(commissionIds));

            return commissionTables;
        }

        private async Task<List<ProductRevenue>> GetProductBranchRevenue(PaysheetDto paySheetDto, List<BranchDto> branchedDto, List<long> branchIds, List<CommissionSalaryRuleValueV2> commissionSalaryRuleValue)
        {

            var branchProductRevenues = new List<ProductRevenue>();
            if (commissionSalaryRuleValue.Any(c => c.FormalityTypes == CommissionSalaryFormalityTypes.BranchCommissionRevenue))
            {
                var isAllBranch = commissionSalaryRuleValue.Any(c => c.IsAllBranch && c.FormalityTypes == CommissionSalaryFormalityTypes.BranchCommissionRevenue);
                branchProductRevenues = await _kiotVietInternalService.GetBranchProductRevenues(
                    paySheetDto.TenantId,
                    _authService.Context.TenantCode,
                    isAllBranch,
                    branchedDto,
                    branchIds,
                    paySheetDto.StartTime.Date,
                    paySheetDto.EndTime.Date.AddDays(1).AddTicks(-1));
            }

            return branchProductRevenues;
        }

        #endregion

        #region Process Payrate

        /// <summary>
        /// Lấy danh sách thiết lập lương cho nhân viên
        /// </summary>
        /// <param name="listEmployeeId"></param>
        /// <param name="paysheetDto"></param>
        /// <param name="context"></param>
        /// <param name="isUpdate"></param>
        /// <param name="payheetCreateOldTime"></param>
        /// <param name="paysheetOldDto">Chỉ có trường hợp chuyển kỳ trong cập nhật mới có dữ liệu</param>
        /// <returns></returns>
        private async Task<List<PayRate>> GetListPayrateProcessAsync(List<long> listEmployeeId, PaysheetDto paysheetDto, IntegrationEventContext context, bool isUpdate, DateTime? payheetCreateOldTime = null, PaysheetDto paysheetOldDto = null)
        {
            try
            {
                var listPayRatedDto = await _mediator.Send(new GetPayRateByEmployeeIdsQuery(listEmployeeId));

                return listPayRatedDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                var eventContext = GetIntegrationEventContext(paysheetDto, context);
                _descriptionError = (int)PaysheetErrorCode.Payrate;
                var paySheetStatues = isUpdate ? (byte)PaysheetStatuses.TemporarySalary : paysheetDto.PaysheetStatus;
                if (payheetCreateOldTime != null)
                {
                    paysheetDto.PaysheetCreatedDate = payheetCreateOldTime;
                }
                if (paysheetOldDto != null)
                {
                    UpdateOldPaysheetDto(paysheetDto, paysheetOldDto);
                }
                var eventAuditLogError = new UpdatePaysheetProcessErrorIntegrationEvent(paysheetDto.Code, paysheetDto.Name,
                    paySheetStatues, "Lỗi khi lấy danh sách thiết lập lương cho nhân viên", paysheetDto, isUpdate);
                eventAuditLogError.SetContext(eventContext);
                _auditLogProcess.SendEventData(eventAuditLogError, nameof(UpdatePaysheetProcessErrorIntegrationEvent));
                throw;
            }
        }
        #endregion

        #region process Allowances

        /// <summary>
        /// Lấy danh sách phụ cấp
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="paySheetDto"></param>
        /// <param name="context"></param>
        /// <param name="isUpdate"></param>
        /// <param name="payheetCreateOldTime"></param>
        /// <param name="paysheetOldDto">Chỉ có trường hợp chuyển kỳ trong cập nhật mới có dữ liệu</param>
        /// <returns></returns>
        private async Task<List<AllowanceDto>> GetAllowanceProcessAsync(int tenantId, PaysheetDto paySheetDto, IntegrationEventContext context, bool isUpdate, DateTime? payheetCreateOldTime = null, PaysheetDto paysheetOldDto = null)
        {
            // Lấy danh sách phụ cấp 
            try
            {
                var allowances = await _mediator.Send(new GetAllowanceByTenantIdQuery(tenantId));

                return allowances;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                var eventContext = GetIntegrationEventContext(paySheetDto, context);
                _descriptionError = (int)PaysheetErrorCode.Allowance;
                var paysheetStatues = isUpdate ? (byte)PaysheetStatuses.TemporarySalary : paySheetDto.PaysheetStatus;
                if (payheetCreateOldTime != null)
                {
                    paySheetDto.PaysheetCreatedDate = payheetCreateOldTime;
                }
                if (paysheetOldDto != null)
                {
                    UpdateOldPaysheetDto(paySheetDto, paysheetOldDto);
                }
                var eventAuditLog = new UpdatePaysheetProcessErrorIntegrationEvent(paySheetDto.Code, paySheetDto.Name,
                    paysheetStatues, "Lỗi khi lấy danh sách phụ cấp", paySheetDto, isUpdate);
                eventAuditLog.SetContext(eventContext);
                _auditLogProcess.SendEventData(eventAuditLog, nameof(UpdatePaysheetProcessErrorIntegrationEvent));
                throw;
            }
        }
        #endregion

        #region Process Deductions

        /// <summary>
        /// Lấy danh sách giảm trừ
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="paysheetDto"></param>
        /// <param name="context"></param>
        /// <param name="isUpdate"></param>
        /// <param name="payheetCreateOldTime"></param>
        /// <param name="paysheetOldDto">Chỉ có trường hợp chuyển kỳ trong cập nhật mới có dữ liệu</param>
        /// <returns></returns>
        private async Task<List<DeductionDto>> GetDeductionProcessAsync(int tenantId, PaysheetDto paysheetDto, IntegrationEventContext context, bool isUpdate, DateTime? payheetCreateOldTime = null, PaysheetDto paysheetOldDto = null)
        {
            try
            {
                var deductions = await _mediator.Send(new GetAllDeductionByTenantDQuery(tenantId));

                return deductions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _descriptionError = (int)PaysheetErrorCode.Deduction;
                var eventContext = GetIntegrationEventContext(paysheetDto, context);
                var paysheetStatues = isUpdate ? (byte)PaysheetStatuses.TemporarySalary : paysheetDto.PaysheetStatus;
                if (payheetCreateOldTime != null)
                {
                    paysheetDto.PaysheetCreatedDate = payheetCreateOldTime;
                }
                if (paysheetOldDto != null)
                {
                    UpdateOldPaysheetDto(paysheetDto, paysheetOldDto);
                }
                var eventAuditLog = new UpdatePaysheetProcessErrorIntegrationEvent(paysheetDto.Code, paysheetDto.Name,
                    paysheetStatues, "Lỗi khi lấy danh sách giảm trừ", paysheetDto, isUpdate);
                eventAuditLog.SetContext(eventContext);
                _auditLogProcess.SendEventData(eventAuditLog, nameof(UpdatePaysheetProcessErrorIntegrationEvent));
                throw;
            }
        }
        #endregion

        #region Process Clockings

        /// <summary>
        /// Lấy danh sách clocking bao gồm các trạng thái đã chấm công (chấm công vào, chấm công ra) và chấm công coi là ngày nghỉ (holiday, nghỉ phép) 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="listEmployeeId"></param>
        /// <param name="branchId"></param>
        /// <param name="paysheetDto"></param>
        /// <param name="context"></param>
        /// <param name="isUpdate"></param>
        /// <param name="payheetCreateOldTime"></param>
        /// <param name="paysheetOldDto">Chỉ có trường hợp chuyển kỳ trong cập nhật mới có dữ liệu</param>
        /// <returns>
        /// return four items. They are used to calculate payslip-clocking
        /// Item 1: list clocking
        /// Item 2: list clocking paid
        /// Item 3: list clocking un paid
        /// Item 4: list shift of branch
        /// Item 5: list holiday
        /// Item 6: list clocking-penalize
        /// </returns>
        private async Task<Tuple<List<ClockingDto>, List<ClockingDto>, List<ClockingDto>, List<ShiftDto>, List<HolidayDto>, List<ClockingPenalizeDto>>>
            GetClockingProcessAsync(
                DateTime startTime, DateTime endTime,
                List<long> listEmployeeId, int branchId,
                PaysheetDto paysheetDto, IntegrationEventContext context, bool isUpdate, DateTime? payheetCreateOldTime = null, PaysheetDto paysheetOldDto = null)
        {
            try
            {
                // Get list clocking has check-in, check-out and nghỉ phép
                var listClockingTask = _mediator.Send(new GetClockingForPaySheetQuery(startTime, endTime, listEmployeeId));

                var listShiftTask = _mediator.Send(new GetShiftByBranchIdQuery(branchId));

                var listHolidayTask = _mediator.Send(new GetHolidayForPaySheetQuery(startTime, endTime));

                await Task.WhenAll(listClockingTask, listShiftTask, listHolidayTask);

                var listClockingPenalize = await _mediator.Send(new GetClockingPenalizeForPaySheetQuery(startTime, endTime, listEmployeeId));

                // Get list clocking has un paid
                var unListPaidClocking = listClockingTask.Result.Where(c => c.ClockingPaymentStatus == (byte)ClockingPaymentStatuses.UnPaid).ToList();

                // Get list clocking has paid
                var paidListClocking = listClockingTask.Result.Where(c => c.ClockingPaymentStatus == (byte)ClockingPaymentStatuses.Paid).ToList();

                return new Tuple<List<ClockingDto>, List<ClockingDto>, List<ClockingDto>, List<ShiftDto>, List<HolidayDto>, List<ClockingPenalizeDto>>(
                    listClockingTask.Result, paidListClocking, unListPaidClocking, listShiftTask.Result, listHolidayTask.Result, listClockingPenalize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                var eventContext = GetIntegrationEventContext(paysheetDto, context);
                _descriptionError = (int)PaysheetErrorCode.Clocking;
                var paysheetStatues = isUpdate ? (byte)PaysheetStatuses.TemporarySalary : paysheetDto.PaysheetStatus;
                if (payheetCreateOldTime != null)
                {
                    paysheetDto.PaysheetCreatedDate = payheetCreateOldTime;
                }
                if (paysheetOldDto != null)
                {
                    UpdateOldPaysheetDto(paysheetDto, paysheetOldDto);
                }
                var eventAuditLog = new UpdatePaysheetProcessErrorIntegrationEvent(paysheetDto.Code, paysheetDto.Name,
                    paysheetStatues, "Lỗi khi lấy danh sách chấm công, ngày nghỉ lễ, nghỉ phép", paysheetDto, isUpdate);
                eventAuditLog.SetContext(eventContext);
                _auditLogProcess.SendEventData(eventAuditLog, nameof(UpdatePaysheetProcessErrorIntegrationEvent));
                throw;
            }
        }

        #endregion

        #region Process api interal

        /// <summary>
        /// Lấy tiền đã thanh toán cho nhân viên (UserByRevenueObject).
        /// Lấy hoa hồng tính theo danh sách nhân viên (ProductRevenue)
        /// </summary>
        /// <param name="employees"></param>
        /// <param name="paySheetDto"></param>
        /// <param name="context"></param>
        /// <param name="isUpdate"></param>
        /// <param name="payheetCreateOldTime"></param>
        /// <param name="paysheetOldDto">Chỉ có trường hợp chuyển kỳ trong cập nhật mới có dữ liệu</param>
        /// <returns>
        /// Return two items.
        /// Item 1: List userByRevenueObject
        /// Item 2: List productRevenue
        /// </returns>
        private async Task<Tuple<List<UserByRevenueObject>, List<ProductRevenue>>> GetApiInternalRevenue(
            List<EmployeeDto> employees, PaysheetDto paySheetDto, IntegrationEventContext context, bool isUpdate, DateTime? payheetCreateOldTime = null, PaysheetDto paysheetOldDto = null, SettingObjectDto setting = null)
        {
            try
            {
                var employeeIds = employees.Select(e => e.Id).ToList();
                List<int> branchIds = new List<int>();
                foreach (var employee in employees)
                {
                    branchIds.AddRange(employee.EmployeeBranches.Select(b => b.BranchId));
                }

                var userRevenuesTask = _kiotVietInternalService.GetUserByRevenue(
                    paySheetDto.TenantId,
                    branchIds,
                    employeeIds,
                    paySheetDto.StartTime.Date,
                    paySheetDto.EndTime.Date.AddDays(1).AddTicks(-1));

                var userProductRevenuesTask = _kiotVietInternalService.GetProductRevenueByUser(
                    paySheetDto.TenantId,
                    branchIds,
                    employeeIds,
                    paySheetDto.StartTime.Date,
                    paySheetDto.EndTime.Date.AddDays(1).AddTicks(-1),
                    setting.CommissionSetting);

                await Task.WhenAll(userRevenuesTask, userProductRevenuesTask);

                var userRevenues = userRevenuesTask.Result ?? new List<UserByRevenueObject>();
                var userProductRevenues = userProductRevenuesTask.Result ?? new List<ProductRevenue>();

                userRevenues = userRevenues.GroupBy(x => x.EmployeeId).Select(x => new UserByRevenueObject
                {
                    EmployeeId = x.Key,
                    NetSale = x.Sum(a => a.NetSale),
                    TotalGrossProfit = x.Sum(a => a.TotalGrossProfit)
                }).ToList();

                return new Tuple<List<UserByRevenueObject>, List<ProductRevenue>>(userRevenues, userProductRevenues);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _descriptionError = (int)PaysheetErrorCode.Employee;
                var eventContext = GetIntegrationEventContext(paySheetDto, context);
                var paysheetStatues = isUpdate ? (byte)PaysheetStatuses.TemporarySalary : paySheetDto.PaysheetStatus;
                if (payheetCreateOldTime != null)
                {
                    paySheetDto.PaysheetCreatedDate = payheetCreateOldTime;
                }
                if (paysheetOldDto != null)
                {
                    UpdateOldPaysheetDto(paySheetDto, paysheetOldDto);
                }
                var eventAuditLog = new UpdatePaysheetProcessErrorIntegrationEvent(paySheetDto.Code, paySheetDto.Name,
                    paysheetStatues, "Lỗi khi lấy Doanh thu cho nhân viên", paySheetDto, isUpdate);
                eventAuditLog.SetContext(eventContext);
                _auditLogProcess.SendEventData(eventAuditLog, nameof(UpdatePaysheetProcessErrorIntegrationEvent));
                throw;
            }
        }

        private async Task<Tuple<List<UserByRevenueObject>, List<ProductRevenue>>> GetApiInternalRevenueCounselor(
            List<EmployeeDto> employees, PaysheetDto paySheetDto, IntegrationEventContext context, bool isUpdate, DateTime? payheetCreateOldTime = null, PaysheetDto paysheetOldDto = null, SettingObjectDto setting = null)
        {
            try
            {
                var employeeIds = employees.Select(e => e.Id).ToList();
                List<int> branchIds = new List<int>();
                foreach (var employee in employees)
                {
                    branchIds.AddRange(employee.EmployeeBranches.Select(b => b.BranchId));
                }

                var userRevenuesTask = _kiotVietInternalService.GetUserByRevenueCounselor(
                    paySheetDto.TenantId,
                    branchIds,
                    employeeIds,
                    paySheetDto.StartTime.Date,
                    paySheetDto.EndTime.Date.AddDays(1).AddTicks(-1));

                var userProductRevenuesTask = _kiotVietInternalService.GetProductRevenueByUserCounselor(
                    paySheetDto.TenantId,
                    branchIds,
                    employeeIds,
                    paySheetDto.StartTime.Date,
                    paySheetDto.EndTime.Date.AddDays(1).AddTicks(-1),
                    setting.CommissionSetting);

                await Task.WhenAll(userRevenuesTask, userProductRevenuesTask);

                var userRevenues = userRevenuesTask.Result ?? new List<UserByRevenueObject>();
                var userProductRevenues = userProductRevenuesTask.Result ?? new List<ProductRevenue>();

                userRevenues = userRevenues.GroupBy(x => x.EmployeeId).Select(x => new UserByRevenueObject
                {
                    EmployeeId = x.Key,
                    NetSale = x.Sum(a => a.NetSale),
                    TotalGrossProfit = x.Sum(a => a.TotalGrossProfit)
                }).ToList();

                return new Tuple<List<UserByRevenueObject>, List<ProductRevenue>>(userRevenues, userProductRevenues);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _descriptionError = (int)PaysheetErrorCode.Employee;
                var eventContext = GetIntegrationEventContext(paySheetDto, context);
                var paysheetStatues = isUpdate ? (byte)PaysheetStatuses.TemporarySalary : paySheetDto.PaysheetStatus;
                if (payheetCreateOldTime != null)
                {
                    paySheetDto.PaysheetCreatedDate = payheetCreateOldTime;
                }
                if (paysheetOldDto != null)
                {
                    UpdateOldPaysheetDto(paySheetDto, paysheetOldDto);
                }
                var eventAuditLog = new UpdatePaysheetProcessErrorIntegrationEvent(paySheetDto.Code, paySheetDto.Name,
                    paysheetStatues, "Lỗi khi lấy Doanh thu cho nhân viên", paySheetDto, isUpdate);
                eventAuditLog.SetContext(eventContext);
                _auditLogProcess.SendEventData(eventAuditLog, nameof(UpdatePaysheetProcessErrorIntegrationEvent));
                throw;
            }
        }

        #endregion

        #region PRIVATE
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paysheetDto"></param>
        /// <param name="paysheetOldDto"></param>
        private void UpdateOldPaysheetDto(PaysheetDto paysheetDto, PaysheetDto paysheetOldDto)
        {
            paysheetDto.Name = paysheetOldDto.Name;
            paysheetDto.TimeOfStandardWorkingDay = paysheetOldDto.TimeOfStandardWorkingDay;
            paysheetDto.WorkingDayNumber = paysheetOldDto.WorkingDayNumber;
            paysheetDto.PaysheetPeriodName = paysheetOldDto.PaysheetPeriodName;
            paysheetDto.StartTime = paysheetOldDto.StartTime;
            paysheetDto.EndTime = paysheetOldDto.EndTime;
        }

        /// <summary>
        /// Lấy danh sách nhân viên
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="paySheetDto"></param>
        /// <param name="context"></param>
        /// <param name="isUpdate"></param>
        /// <param name="paysheetOldDto">Chỉ có trường hợp chuyển kỳ trong cập nhật mới có dữ liệu</param>
        /// <returns>
        /// return two items.
        /// Item 1: list EmployeeDto.
        /// Item 2: list employeeIds 
        /// </returns>
        private async Task<Tuple<List<EmployeeDto>, List<long>>> GetListEmployeesDto(int branchId, PaysheetDto paySheetDto, IntegrationEventContext context, bool isUpdate, DateTime? payheetCreateOldTime = null, PaysheetDto paysheetOldDto = null)
        {
            try
            {
                // Get list employees
                var employees = await _mediator.Send(new GetEmployeeByBranchIdQuery(branchId, 0));

                employees = employees.OrderBy(x => x.Name.Split(' ').Last()).ToList();
                var listEmployeeId = employees.Select(e => e.Id).ToList();

                //get list branch working of employees
                var lstBranchWorking = await _mediator.Send(new GetEmployeeBranchByEmployeeIdsQuery(listEmployeeId));
                employees.ForEach(e =>
                {
                    var lstBranchWithEmployee = lstBranchWorking.Where(x => x.EmployeeId == e.Id).ToList();
                    e.EmployeeBranches = _mapper.Map<List<EmployeeBranch>>(lstBranchWithEmployee);
                });

                return new Tuple<List<EmployeeDto>, List<long>>(employees, listEmployeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (paysheetOldDto != null)
                {
                    UpdateOldPaysheetDto(paySheetDto, paysheetOldDto);
                }
                var eventContext = GetIntegrationEventContext(paySheetDto, context);
                _descriptionError = (int)PaysheetErrorCode.Employee;
                var paysheetStatues = isUpdate ? (byte)PaysheetStatuses.TemporarySalary : paySheetDto.PaysheetStatus;
                if (payheetCreateOldTime != null)
                {
                    paySheetDto.PaysheetCreatedDate = payheetCreateOldTime;
                }
                var eventAuditLog = new UpdatePaysheetProcessErrorIntegrationEvent(paySheetDto.Code, paySheetDto.Name,
                    paysheetStatues, "Lỗi khi lấy thông tin danh sách nhân viên", paySheetDto, isUpdate);
                eventAuditLog.SetContext(eventContext);
                _auditLogProcess.SendEventData(eventAuditLog, nameof(UpdatePaysheetProcessErrorIntegrationEvent));
                throw;
            }
        }

        private async Task UpdatePaySheetPayslips(PaysheetDto paySheetDto,
            List<EmployeeDto> listEmployeeDto,
            List<UserByRevenueObject> userRevenues,
            List<UserByRevenueObject> userCounselorRevenues,
            List<DeductionDto> listDeductionDto,
            List<AllowanceDto> listAllowanceDto,
            List<CommissionDto> listCommissionDto,
            List<ClockingDto> listClockingDto,
            List<ClockingDto> listClockingPaid,
            List<ClockingDto> listClockingUnPaid,
            List<PayRate> payRatesDto,
            List<HolidayDto> listHolidayDto,
            List<ShiftDto> listShiftDto,
            List<ProductRevenue> userProductRevenues,
            List<ProductRevenue> userCounselorProductRevenues,
            List<ProductRevenue> branchProductRevenues,
            List<ClockingPenalizeDto> clockingPenalizesDto,
            IntegrationEventContext context,
            bool isUpdate,
            SettingObjectDto settingObjectDto)
        {
            try
            {
                var result = await _mediator.Send(new UpdatePaySheetPayslipsCommand(
                        paySheetDto, listEmployeeDto, userRevenues, userCounselorRevenues, listDeductionDto, listAllowanceDto, listCommissionDto,
                        listClockingDto, listClockingPaid, listClockingUnPaid,
                        payRatesDto, listHolidayDto, listShiftDto, userProductRevenues, userCounselorProductRevenues, branchProductRevenues, clockingPenalizesDto, settingObjectDto));

                var descriptions = string.Format(Message.create_successed, Label.paysheet.ToLower());
                if (_useMqtt)
                {
                    await PublishMessageAsync(
                    paySheetDto, EventTypeStatic.AutoLoadingAndUpdatePaySheetIntegrationSocket,
                    descriptions, result.PaysheetStatus, (int)HttpStatusCode.OK, context);
                }

                var eventContext = GetIntegrationEventContext(paySheetDto, context);
                var eventAuditPaySheet = new UpdatePaysheetProcessIntegrationEvent(paySheetDto.Id, paySheetDto.Code, paySheetDto.Name, paySheetDto.PaysheetStatus);
                eventAuditPaySheet.SetContext(eventContext);
                _auditLogProcess.SendEventData(eventAuditPaySheet, nameof(UpdatePaysheetProcessIntegrationEvent));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                var eventContext = GetIntegrationEventContext(paySheetDto, context);
                var eventAuditLog = new UpdatePaysheetProcessErrorIntegrationEvent(paySheetDto.Code, paySheetDto.Name,
                    (byte)PaysheetStatuses.TemporarySalary, "Lỗi khi tạo phiếu lương", paySheetDto, isUpdate);
                eventAuditLog.SetContext(eventContext);
                _auditLogProcess.SendEventData(eventAuditLog, nameof(UpdatePaysheetProcessErrorIntegrationEvent));
                throw;
            }
        }

        private async Task<PaysheetDto> CreatePaySheetPayslips(PaysheetDto paySheetDto,
            List<EmployeeDto> listEmployeeDto,
            List<UserByRevenueObject> userRevenues,
            List<UserByRevenueObject> userCounselorRevenues,
            List<DeductionDto> listDeductionDto,
            List<AllowanceDto> listAllowanceDto,
            List<CommissionDto> listCommissionDto,
            List<ClockingDto> listClockingDto,
            List<ClockingDto> listClockingPaid,
            List<ClockingDto> listClockingUnPaid,
            List<PayRate> payRatesDto,
            List<HolidayDto> listHolidayDto,
            List<ShiftDto> listShiftDto,
            List<ProductRevenue> userProductRevenues,
            List<ProductRevenue> userCounselorProductRevenues,
            List<ProductRevenue> branchProductRevenues,
            List<ClockingPenalizeDto> clockingPenalizesDto,
            IntegrationEventContext context,
            bool isUpdate,
            SettingObjectDto settingObjectDto)
        {
            try
            {
                var result = await _mediator.Send(new CreatePaySheetPayslipCommand(
                        paySheetDto, listEmployeeDto, userRevenues, userCounselorRevenues, listDeductionDto, listAllowanceDto, listCommissionDto,
                        listClockingDto, listClockingPaid, listClockingUnPaid,
                        payRatesDto, listHolidayDto, listShiftDto, userProductRevenues, userCounselorProductRevenues, branchProductRevenues,
                        clockingPenalizesDto, settingObjectDto));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                var eventContext = GetIntegrationEventContext(paySheetDto, context);
                var eventAuditLog = new UpdatePaysheetProcessErrorIntegrationEvent(paySheetDto.Code, paySheetDto.Name,
                    paySheetDto.PaysheetStatus, "Lỗi khi tạo phiếu lương", paySheetDto, isUpdate);
                eventAuditLog.SetContext(eventContext);
                _auditLogProcess.SendEventData(eventAuditLog, nameof(UpdatePaysheetProcessErrorIntegrationEvent));
                throw;
            }
        }
        #endregion
    }
}
