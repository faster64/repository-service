using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Application.Validators.PaysheetValidators;
using KiotVietTimeSheet.Application.Validators.PayslipValidator;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Helpers;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Application.Commands.CompletePaysheet
{
    public class CompletePaysheetCommandHandler : BaseCommandHandler,
        IRequestHandler<CompletePaysheetCommand, PaysheetDto>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IPaysheetWriteOnlyRepository _paysheetWriteOnlyRepository;
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IPayRateReadOnlyRepository _payRateReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ICompleteSalaryClockingDomainService _completeSalaryClockingDomainService;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CompletePaysheetCommandHandler> _logger;

        public CompletePaysheetCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IPaysheetWriteOnlyRepository paysheetWriteOnlyRepository,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IPayRateReadOnlyRepository payRateReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            ICompleteSalaryClockingDomainService completeSalaryClockingDomainService,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            ILogger<CompletePaysheetCommandHandler> logger
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _eventDispatcher = eventDispatcher;
            _paysheetWriteOnlyRepository = paysheetWriteOnlyRepository;
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _payRateReadOnlyRepository = payRateReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _completeSalaryClockingDomainService = completeSalaryClockingDomainService;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _logger = logger;
        }

        public async Task<PaysheetDto> Handle(CompletePaysheetCommand request, CancellationToken cancellationToken)
        {
            var paySheetDtoItem = request.Paysheet;
            var existingPaySheet = await _paysheetWriteOnlyRepository.FindBySpecificationAsync(
                new FindPaysheetByIdSpec(paySheetDtoItem.Id).And(
                    new FindPaysheetByStatusSpec((byte)PaysheetStatuses.TemporarySalary)), "Payslips");

            var oldPaySheet = existingPaySheet.CreateCopy();

            if (existingPaySheet == null)
            {
                NotifyPaySheetInDbIsNotExists();
                return null;
            }

            if (request.Paysheet.Payslips != null && request.Paysheet.Payslips.Any())
            {
                var employeeIds = request.Paysheet.Payslips.Select(p => p.EmployeeId).ToList();
                var (isHasDeleted, deletedName) =
                    await _payRateReadOnlyRepository.CheckPayRateTemplateDeleteByEmployeeIds(employeeIds);
                if (isHasDeleted)
                {
                    await _eventDispatcher.FireEvent(new DomainNotification(nameof(Paysheet),
                        string.Format(Message.delPayRateTemplateValidate, deletedName)));
                    return null;
                }
            }

            var validatorResult = await (new HasCanceledPayslipValidator(paySheetDtoItem.Payslips).ValidateAsync(existingPaySheet));
            if (!validatorResult.IsValid)
            {
                NotifyValidationErrors(typeof(Paysheet), validatorResult.Errors);
                return null;
            }

            // Kiểm tra xem bảng hoa hồng đã bị xóa hay bị ngừng áp dụng
            var commissionParamUseCommissionTable = paySheetDtoItem.Payslips.Where(x =>
                    x.CommissionSalaryRuleParam != null &&
                    x.CommissionSalaryRuleParam.Type == CommissionSalaryTypes.WithTotalCommission &&
                    x.CommissionSalaryRuleParam.CommissionParams != null &&
                    x.CommissionSalaryRuleParam.CommissionParams.Any(p => p.CommissionTable != null))
                .Select(x => x.CommissionSalaryRuleParam)
                .ToList();

            foreach (var commissionSalaryRuleValueV2 in commissionParamUseCommissionTable)
            {
                var validatorCommissionResult = await (new CommissionIsDeleteOrUnActiveValidator(_commissionReadOnlyRepository).ValidateAsync(commissionSalaryRuleValueV2, cancellationToken));
                if (validatorCommissionResult.IsValid) continue;
                NotifyValidationErrors(typeof(Paysheet), validatorCommissionResult.Errors);
                return null;
            }

            existingPaySheet.Payslips = existingPaySheet.Payslips
                ?.Where(p => p.PayslipStatus != (byte)PayslipStatuses.Void && p.PayslipStatus != (byte)PayslipStatuses.PaidSalary).ToList();
            var isChangeCode = existingPaySheet.Code != paySheetDtoItem.Code;
            await UpdatePaySheetFromDto(paySheetDtoItem, existingPaySheet, true);

            validatorResult = await ValidatePaySheetAndPayslipCompleteCommand(existingPaySheet, isChangeCode);
            if (!validatorResult.IsValid)
            {
                NotifyValidationErrors(typeof(Paysheet), validatorResult.Errors);
                return null;
            }
            // Nếu có hủy phiếu lương, sẽ kiểm tra phiếu lương đó có phiếu thanh toán k
            var payslipsNeedCancel = existingPaySheet.Payslips?.Where(p => p.PayslipStatus == (byte)PayslipStatuses.Void).ToList();
            var checkPayslipPayment = await CheckPayslipPayment(request, payslipsNeedCancel);
            if (!checkPayslipPayment) return null;

            existingPaySheet.Complete();

            // Store db
            await _paysheetWriteOnlyRepository.StoreAsync(existingPaySheet, true);
            #region BOOK-7671 bug ko tái hiện được đặt log để theo dõi
            var temporarySalaryPayslips = existingPaySheet.Payslips?.Where(p => p.PayslipStatus == (byte)PayslipStatuses.TemporarySalary).ToList();

            LogPayslipStatus(temporarySalaryPayslips, 1);
            #endregion
            await _completeSalaryClockingDomainService.CompletePaysheetForClockingsAsync(existingPaySheet);

            LogPayslipStatus(temporarySalaryPayslips, 2);

            #region Tạo phiếu thanh toán

            await MakePayslipPaymentsForPaySheetCompleteCommandAsync(paySheetDtoItem, existingPaySheet, true);

            LogPayslipStatus(temporarySalaryPayslips, 3);

            #endregion

            // Hủy phiếu thanh toán, phiếu hoàn trả tạm ứng
            await CancelPayslipPayment(payslipsNeedCancel, request.IsCancelPayment);

            LogPayslipStatus(temporarySalaryPayslips, 4);

            // Audit Log
            await AddAuditLogCompletePaysheet(existingPaySheet, oldPaySheet);

            LogPayslipStatus(temporarySalaryPayslips, 5);

            await _paysheetWriteOnlyRepository.UnitOfWork.CommitAsync();

            var retPaySheetCompleteCommand = _mapper.Map<PaysheetDto>(existingPaySheet);
            retPaySheetCompleteCommand.Payslips = _mapper.Map<List<PayslipDto>>(existingPaySheet.Payslips);
            return retPaySheetCompleteCommand;
        }

        public async Task<bool> CheckPayslipPayment(CompletePaysheetCommand request, List<Payslip> payslipsNeedCancel)
        {
            if (!request.IsCheckPayslipPayment || payslipsNeedCancel == null || !payslipsNeedCancel.Any()) return true;

            var payslipPayments =
                await _kiotVietServiceClient.GetPayslipPaymentsAsync(
                    new GetPayslipPaymentsReq
                    {
                        PayslipIds = payslipsNeedCancel.Select(p => p.Id).ToList(),
                        Status = (byte)PayslipPaymentStatuses.Paid,
                        WithAllocation = false
                    });

            return payslipPayments == null || !payslipPayments.Data.Any();
        }

        public async Task AddAuditLogCompletePaysheet(Paysheet existingPaySheet, Paysheet oldPaySheet)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedPaysheetIntegrationEvent(oldPaySheet, existingPaySheet));
        }

        private async Task UpdatePaySheetFromDto(PaysheetDto paySheetDto, Paysheet existingPaySheetCompleteCommand, bool isComplete = false)
        {
            var status = isComplete ? (byte)PaysheetStatuses.PaidSalary : (byte)PaysheetStatuses.TemporarySalary;
            existingPaySheetCompleteCommand.Update(
                paySheetDto.Code,
                paySheetDto.Name,
                paySheetDto.Note,
                paySheetDto.WorkingDayNumber,
                paySheetDto.SalaryPeriod,
                paySheetDto.StartTime,
                paySheetDto.EndTime,
                paySheetDto.PaysheetPeriodName,
                paySheetDto.CreatorBy,
                paySheetDto.PaysheetCreatedDate,
                status);

            if (existingPaySheetCompleteCommand.Payslips != null && existingPaySheetCompleteCommand.Payslips.Any())
            {
                if (paySheetDto.Payslips == null || !paySheetDto.Payslips.Any())
                {
                    existingPaySheetCompleteCommand.Payslips.ForEach(payslip => payslip.CancelWithoutEvent());
                }
                else
                {
                    existingPaySheetCompleteCommand.Payslips = await CreateOrUpdateOrCancelPayslipFromDto(paySheetDto.Payslips,
                        existingPaySheetCompleteCommand.Payslips, paySheetDto.Id, existingPaySheetCompleteCommand.CreatorBy, existingPaySheetCompleteCommand.PaysheetCreatedDate);
                }
            }
            else
            {
                if (paySheetDto.Payslips == null || !paySheetDto.Payslips.Any()) return;
                existingPaySheetCompleteCommand.Payslips = await CreateOrUpdateOrCancelPayslipFromDto(paySheetDto.Payslips,
                    existingPaySheetCompleteCommand.Payslips, paySheetDto.Id);
            }
        }

        private async Task<ValidationResult> ValidatePaySheetAndPayslipCompleteCommand(Paysheet existingPaySheet, bool isChangeCode)
        {
            // Lấy thông tin phiếu thanh toán để validate với thời gian lập bảng lương
            var listPayslipPayment =
                await _kiotVietServiceClient.GetPayslipPaymentsAsync(
                    new GetPayslipPaymentsReq
                    {
                        PaysheetId = existingPaySheet.Id
                    });

            var firstPayslipPaymentItem = listPayslipPayment?.Data?.Where(p => p.Status != (byte)PayslipPaymentStatuses.Void)
                .OrderBy(p => p.TransDate).FirstOrDefault();

            // PaySheet validator
            var validatorResult =
                await (new CreateOrUpdatePaysheetValidator(_paysheetWriteOnlyRepository, existingPaySheet.Payslips, firstPayslipPaymentItem,
                    isChangeCode).ValidateAsync(existingPaySheet));
            if (!validatorResult.IsValid)
            {
                return validatorResult;
            }

            // Payslip validator
            validatorResult =
                await (new CreateOrUpdatePayslipValidator(_payslipReadOnlyRepository, _employeeReadOnlyRepository)
                    .ValidateAsync(existingPaySheet.Payslips));
            return validatorResult;
        }

        private async Task MakePayslipPaymentsForPaySheetCompleteCommandAsync(PaysheetDto paySheetDtoItem, Paysheet existingPaySheetItem, bool isComplete = false)
        {
            if (paySheetDtoItem.Payslips != null && paySheetDtoItem.Payslips.Any())
            {
                paySheetDtoItem.Payslips.ForEach(ps =>
                {
                    var createdPayslipCompleteCommand = existingPaySheetItem.Payslips.FirstOrDefault(p => p.EmployeeId == ps.EmployeeId);
                    if (createdPayslipCompleteCommand == null) return;
                    ps.Id = createdPayslipCompleteCommand.Id;
                    ps.Code = createdPayslipCompleteCommand.Code;
                    ps.PaysheetId = existingPaySheetItem.Id;
                    ps.PayslipCreatedDate = createdPayslipCompleteCommand.PayslipCreatedDate;
                });

                await _kiotVietServiceClient.MakePayslipPaymentsForPaysheetAsync(new MakePayslipPaymentsForPaysheetReq
                {
                    Paysheet = paySheetDtoItem,
                    IsComplete = isComplete
                });
            }
        }

        private async Task CancelPayslipPayment(List<Payslip> payslipsNeedCancel, bool isCancelPayment, bool cancelPaySheet = false)
        {
            if (payslipsNeedCancel != null && payslipsNeedCancel.Any())
            {
                await _kiotVietServiceClient.VoidPayslipPaymentsAsync(
                    new VoidPayslipPaymentReq
                    {
                        Payslips = _mapper.Map<List<PayslipDto>>(payslipsNeedCancel),
                        IsVoidPayslipPayment = isCancelPayment,
                        IsCancelPaysheet = cancelPaySheet
                    });
            }
        }

        // Dùng với case có payslip dto, có thể có hoặc k payslip
        private async Task<List<Payslip>> CreateOrUpdateOrCancelPayslipFromDto(List<PayslipDto> payslipsDto, List<Payslip> payslips, long paySheetId,
            long? payslipCreatedBy = null, DateTime? payslipCreatedDate = null)
        {
            var employeeIds = payslipsDto.Select(p => p.EmployeeId).ToList();
            var payRatesCompleteCommand = await _payRateReadOnlyRepository.GetBySpecificationAsync(new FindPayRateByEmployeeIdsSpec(employeeIds), true);
            if (payslips == null || !payslips.Any())
            {
                payslips = payslipsDto
                    .Where(payslipDto => payRatesCompleteCommand.Exists(p => p.EmployeeId == payslipDto.EmployeeId))
                    .Select(payslipDto =>
                    {
                        var payRateOld = payslipDto.PayslipDetails.Select(x => new PayRateDetail(x.RuleType, x.RuleValue)).ToList();
                        var rules = SalaryRuleHelpers.GetRulesFromObjectByRuleParamAndPayRateDetail(payslipDto,
                            payRateOld);
                        var payslip = new Payslip(
                            payslipDto.EmployeeId, paySheetId,
                            payslipDto.MainSalary, payslipDto.Allowance,
                            payslipDto.Deduction,
                            payslipDto.CommissionSalary, payslipDto.OvertimeSalary,
                            payslipDto.Bonus, (byte)PayslipStatuses.TemporarySalary,
                            payslipDto.Code, payslipDto.Note,
                            payslipCreatedBy, payslipCreatedDate, rules
                        );

                        payslip.PayslipClockingPenalizes = payslipDto.PayslipClockingPenalizes?.Select(p =>
                                new PayslipClockingPenalize(p.PayslipId, p.ClockingId, p.PenalizeId, p.Value, p.TimesCount, p.MoneyType, p.ClockingPenalizeCreated, p.ShiftId))
                            .ToList();

                        payslip.PayslipPenalizes = payslipDto.PayslipPenalizes?.Select(p =>
                                new PayslipPenalize(p.PayslipId, p.PenalizeId, p.Value, p.TimesCount, p.MoneyType, p.IsActive))
                            .ToList();

                        return payslip;
                    })
                    .ToList();
                return payslips;
            }

            var payslipsNeedCancelInCompletePaySheet =
                payslips.Where(p => !payslipsDto.Select(pd => pd.EmployeeId).ToList().Contains(p.EmployeeId)).ToList();
            payslipsNeedCancelInCompletePaySheet.ForEach(p => p.CancelWithoutEvent());
            var newPayslipsCompleteCommand = payslipsNeedCancelInCompletePaySheet;
            foreach (var payslipDto in payslipsDto)
            {
                var payRateCompleteCommand = payRatesCompleteCommand.FirstOrDefault(p => p.EmployeeId == payslipDto.EmployeeId);
                var payslipCompleteCommand = payslips.FirstOrDefault(p => p.EmployeeId == payslipDto.EmployeeId);
                if (payRateCompleteCommand == null)
                {
                    if (payslipCompleteCommand == null) continue;
                    payslipCompleteCommand.CancelWithoutEvent();
                    newPayslipsCompleteCommand.Add(payslipCompleteCommand);
                    continue;
                }
                var payRateOld = payslipDto.PayslipDetails.Select(x => new PayRateDetail(x.RuleType, x.RuleValue)).ToList();
                AllowanceRuleHelpers.AddPayRateDetailByAllowance(payRateOld, payslipDto.AllowanceRuleParam?.Allowances);
                var rules = SalaryRuleHelpers.GetRulesFromObjectByRuleParamAndPayRateDetail(payslipDto, payRateOld);
                if (payslipCompleteCommand != null)
                {
                    payslipCompleteCommand.Update(
                        payslipDto.MainSalary,
                        payslipDto.Allowance,
                        payslipDto.Deduction,
                        payslipDto.CommissionSalary,
                        payslipDto.OvertimeSalary,
                        payslipDto.Bonus,
                        (byte)PayslipStatuses.TemporarySalary,
                        payslipDto.Code,
                        payslipDto.Note,
                        payslipCreatedBy,
                        payslipCreatedDate,
                        rules);
                }
                else
                {
                    payslipCompleteCommand = new Payslip(
                        payslipDto.EmployeeId,
                        paySheetId,
                        payslipDto.MainSalary,
                        payslipDto.Allowance,
                        payslipDto.Deduction,
                        payslipDto.CommissionSalary,
                        payslipDto.OvertimeSalary,
                        payslipDto.Bonus,
                        (byte)PayslipStatuses.TemporarySalary,
                        payslipDto.Code,
                        payslipDto.Note,
                        payslipCreatedBy,
                        payslipCreatedDate,
                        rules
                    );
                }

                payslipCompleteCommand.PayslipPenalizes = payslipDto.PayslipPenalizes?.Select(p =>
                        new PayslipPenalize(p.PayslipId, p.PenalizeId, p.Value, p.TimesCount, p.MoneyType, p.IsActive))
                    .ToList();

                payslipCompleteCommand.PayslipClockingPenalizes = payslipDto.PayslipClockingPenalizes?.Select(p =>
                        new PayslipClockingPenalize(p.PayslipId, p.ClockingId, p.PenalizeId, p.Value, p.TimesCount, p.MoneyType, p.ClockingPenalizeCreated, p.ShiftId))
                    .ToList();

                newPayslipsCompleteCommand.Add(payslipCompleteCommand);
            }

            return newPayslipsCompleteCommand;
        }

        private void NotifyPaySheetInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Paysheet).Name, string.Format(Message.not_exists, Label.paysheet)));
        }

        private void LogPayslipStatus(List<Payslip> temporarySalaryPayslips, int step)
        {
            if (temporarySalaryPayslips != null && temporarySalaryPayslips.Any())
            {
                _logger.LogInformation($"Complete paysheet step {step} {temporarySalaryPayslips.ToSafeJson()}");
            }
        }
    }
}
