using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Application.Validators.PaysheetValidators;
using KiotVietTimeSheet.Application.Validators.PayslipValidator;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
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

namespace KiotVietTimeSheet.Application.Commands.UpdatePaysheet
{
    public class UpdatePaysheetCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdatePaysheetCommand, PaysheetDto>
    {
        private readonly IMapper _mapper;
        private readonly IPaysheetWriteOnlyRepository _paysheetWriteOnlyRepository;
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IPayRateReadOnlyRepository _payRateReadOnlyRepository;
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IEventDispatcher _eventDispatcher;

        public UpdatePaysheetCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IPaysheetWriteOnlyRepository paysheetWriteOnlyRepository,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IPayRateReadOnlyRepository payRateReadOnlyRepository,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _paysheetWriteOnlyRepository = paysheetWriteOnlyRepository;
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _payRateReadOnlyRepository = payRateReadOnlyRepository;
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public async Task<PaysheetDto> Handle(UpdatePaysheetCommand request, CancellationToken cancellationToken)
        {
            var paysheetDto = request.Paysheet;
            var existingPaySheet = await _paysheetWriteOnlyRepository.FindBySpecificationAsync(
                new FindPaysheetByIdSpec(paysheetDto.Id).And(
                    new FindPaysheetByStatusSpec((byte)PaysheetStatuses.TemporarySalary)), "Payslips");

            if (existingPaySheet == null)
            {
                NotifyPaysheetInDbIsNotExists();
                return null;
            }

            if (existingPaySheet.PaysheetStatus == (byte)PaysheetStatuses.Pending)
            {
                NotifyValidationErrors(typeof(CreateOrUpdatePaysheetValidator), new List<string>() { Message.paysheet_has_pending });
                return null;
            }

            var isDuplicateDeductionName = CheckDuplicateDeductionName(paysheetDto.Payslips);
            if (isDuplicateDeductionName)
            {
                NotifyDedcutionNameInDbIsExists();
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

            var oldPaySheet = existingPaySheet.CreateCopy();

            var isCreate = existingPaySheet.IsDraft;

            var validatorResult = await (new HasCanceledPayslipValidator(paysheetDto.Payslips).ValidateAsync(existingPaySheet));
            if (!validatorResult.IsValid)
            {
                NotifyValidationErrors(typeof(Paysheet), validatorResult.Errors);
                return null;
            }
            // Kiểm tra xem bảng hoa hồng đã bị xóa hay bị ngừng áp dụng
            var commissionParamUseCommisisonTable = paysheetDto.Payslips.Where(x =>
                x.CommissionSalaryRuleParam != null &&
                x.CommissionSalaryRuleParam.Type == CommissionSalaryTypes.WithTotalCommission &&
                x.CommissionSalaryRuleParam.CommissionParams != null &&
                x.CommissionSalaryRuleParam.CommissionParams.Any(p => p.CommissionTable != null))
                .Select(x => x.CommissionSalaryRuleParam)
                .ToList();

            foreach (var commissionSalaryRuleValueV2 in commissionParamUseCommisisonTable)
            {
                var validatorCommissionResult = await (new CommissionIsDeleteOrUnActiveValidator(_commissionReadOnlyRepository).ValidateAsync(commissionSalaryRuleValueV2));
                if (validatorCommissionResult.IsValid) continue;
                NotifyValidationErrors(typeof(Paysheet), validatorCommissionResult.Errors);
                return null;
            }

            existingPaySheet.Payslips = existingPaySheet.Payslips
                ?.Where(p => p.PayslipStatus != (byte)PayslipStatuses.Void && p.PayslipStatus != (byte)PayslipStatuses.PaidSalary).ToList();
            var isChangeCode = existingPaySheet.Code != paysheetDto.Code;
            await UpdatePaySheetFromDto(paysheetDto, existingPaySheet, isCreate);

            validatorResult = await ValidatePaysheetAndPayslip(existingPaySheet, isChangeCode, isCreate);
            if (!validatorResult.IsValid)
            {
                NotifyValidationErrors(typeof(Paysheet), validatorResult.Errors);
                return null;
            }

            // Nếu có hủy phiếu lương, sẽ kiểm tra phiếu lương đó có phiếu thanh toán k
            var payslipsNeedCancel = existingPaySheet.Payslips?.Where(p => p.PayslipStatus == (byte)PayslipStatuses.Void).ToList();

            var checkPayslipPayment = await CheckPayslipPayment(request, isCreate, payslipsNeedCancel);

            if (!checkPayslipPayment) return null;

            await _paysheetWriteOnlyRepository.StoreAsync(existingPaySheet, true);

            #region Tạo phiếu thanh toán phiếu lương

            await MakePayslipPaymentsForPaysheetAsync(paysheetDto, existingPaySheet, false, isCreate);

            #endregion

            // Hủy phiếu thanh toán, phiếu hoàn trả tạm ứng
            await CancelPayslipPayment(payslipsNeedCancel, request.IsCancelPayment);

            // Audit Log

            await AddAuditUpdatePaySheet(existingPaySheet, oldPaySheet);

            await _paysheetWriteOnlyRepository.UnitOfWork.CommitAsync();

            var retPaysheet = _mapper.Map<PaysheetDto>(existingPaySheet);
            retPaysheet.Payslips = _mapper.Map<List<PayslipDto>>(existingPaySheet.Payslips);
            return retPaysheet;
        }

        public async Task<bool> CheckPayslipPayment(UpdatePaysheetCommand request, bool isCreate, List<Payslip> payslipsNeedCancel)
        {
            if (request.IsCheckPayslipPayment && !isCreate && payslipsNeedCancel != null && payslipsNeedCancel.Any())
            {
                var payslipPayments =
                    await _kiotVietServiceClient.GetPayslipPaymentsAsync(
                        new GetPayslipPaymentsReq
                        {
                            PayslipIds = payslipsNeedCancel.Select(p => p.Id).ToList(),
                            Status = (byte)PayslipPaymentStatuses.Paid,
                            WithAllocation = false
                        });
                if (payslipPayments != null && payslipPayments.Data.Any()) return false;
            }

            return true;
        }

        private async Task AddAuditUpdatePaySheet(Paysheet existingPaySheet, Paysheet oldPaySheet)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedPaysheetIntegrationEvent(oldPaySheet, existingPaySheet));
        }

        private async Task UpdatePaySheetFromDto(PaysheetDto paysheetDto, Paysheet existingPaysheet, bool isCreate, bool isComplete = false)
        {
            var status = isComplete ? (byte)PaysheetStatuses.PaidSalary : (byte)PaysheetStatuses.TemporarySalary;
            existingPaysheet.Update(
                paysheetDto.Code,
                paysheetDto.Name,
                paysheetDto.Note,
                paysheetDto.WorkingDayNumber,
                paysheetDto.SalaryPeriod,
                paysheetDto.StartTime,
                paysheetDto.EndTime,
                paysheetDto.PaysheetPeriodName,
                paysheetDto.CreatorBy,
                paysheetDto.PaysheetCreatedDate,
                status);

            if (existingPaysheet.Payslips != null && existingPaysheet.Payslips.Any())
            {
                if (paysheetDto.Payslips == null || !paysheetDto.Payslips.Any())
                {
                    if (!isCreate)
                    {
                        existingPaysheet.Payslips.ForEach(payslip => payslip.CancelWithoutEvent());
                    }
                    else
                    {
                        existingPaysheet.Payslips = null;
                    }
                }
                else
                {
                    existingPaysheet.Payslips = await CreateOrUpdateOrCancelPayslipFromDto(paysheetDto.Payslips,
                        existingPaysheet.Payslips, paysheetDto.Id, existingPaysheet.CreatorBy, existingPaysheet.PaysheetCreatedDate);
                }
            }
            else
            {
                if (paysheetDto.Payslips == null || !paysheetDto.Payslips.Any()) return;
                existingPaysheet.Payslips = await CreateOrUpdateOrCancelPayslipFromDto(paysheetDto.Payslips,
                    existingPaysheet.Payslips, paysheetDto.Id);
            }
        }

        private async Task<List<Payslip>> CreateOrUpdateOrCancelPayslipFromDto(List<PayslipDto> listPayslipDto, List<Payslip> payslips, long paysheetId,
            long? payslipCreatedBy = null, DateTime? payslipCreatedDate = null)
        {
            var employeeIds = listPayslipDto.Select(p => p.EmployeeId).ToList();
            var payRates = await _payRateReadOnlyRepository.GetBySpecificationAsync(new FindPayRateByEmployeeIdsSpec(employeeIds), true);
            if (payslips == null || !payslips.Any())
            {
                payslips = listPayslipDto
                    .Where(payslipDto => payRates.Exists(p => p.EmployeeId == payslipDto.EmployeeId))
                    .Select(payslipDto =>
                    {
                        var payRateOld = payslipDto.PayslipDetails.Select(x => new PayRateDetail(x.RuleType, x.RuleValue)).ToList();
                        var rules = SalaryRuleHelpers.GetRulesFromObjectByRuleParamAndPayRateDetail(payslipDto,
                            payRateOld);
                        var payslip = new Payslip(
                            payslipDto.EmployeeId,
                            paysheetId,
                            payslipDto.MainSalary,
                            payslipDto.Allowance,
                            payslipDto.Deduction,
                            payslipDto.CommissionSalary,
                            payslipDto.OvertimeSalary,
                            payslipDto.Bonus,
                            (byte)PayslipStatuses.TemporarySalary,
                            payslipDto.Code, payslipDto.Note, payslipCreatedBy, payslipCreatedDate,
                            rules
                        );

                        payslip.PayslipPenalizes = payslipDto.PayslipPenalizes?.Select(p =>
                                new PayslipPenalize(p.PayslipId, p.PenalizeId, p.Value, p.TimesCount, p.MoneyType, p.IsActive))
                            .ToList();

                        payslip.PayslipClockingPenalizes = payslipDto.PayslipClockingPenalizes?.Select(p =>
                                new PayslipClockingPenalize(p.PayslipId, p.ClockingId, p.PenalizeId, p.Value, p.TimesCount, p.MoneyType, p.ClockingPenalizeCreated, p.ShiftId))
                            .ToList();

                        return payslip;

                    })
                    .ToList();
                return payslips;
            }

            var payslipsNeedCancel =
                payslips.Where(p => !listPayslipDto.Select(pd => pd.EmployeeId).ToList().Contains(p.EmployeeId)).ToList();
            payslipsNeedCancel.ForEach(p => p.CancelWithoutEvent());
            var newPayslips = payslipsNeedCancel;
            foreach (var payslipDto in listPayslipDto)
            {
                var payRate = payRates.FirstOrDefault(p => p.EmployeeId == payslipDto.EmployeeId);
                var payslip = payslips.FirstOrDefault(p => p.EmployeeId == payslipDto.EmployeeId);
                if (payRate == null)
                {
                    if (payslip == null) continue;
                    payslip.CancelWithoutEvent();
                    newPayslips.Add(payslip);
                    continue;
                }
                var payRateOld = payslipDto.PayslipDetails.Select(x => new PayRateDetail(x.RuleType, x.RuleValue)).ToList();
                AllowanceRuleHelpers.AddPayRateDetailByAllowance(payRateOld, payslipDto.AllowanceRuleParam?.Allowances);
                var listRule = SalaryRuleHelpers.GetRulesFromObjectByRuleParamAndPayRateDetail(payslipDto, payRateOld);
                if (payslip != null)
                {
                    payslip.Update(
                        payslipDto.MainSalary, payslipDto.Allowance, payslipDto.Deduction,
                        payslipDto.CommissionSalary, payslipDto.OvertimeSalary, payslipDto.Bonus, (byte)PayslipStatuses.TemporarySalary,
                        payslipDto.Code, payslipDto.Note, payslipCreatedBy, payslipCreatedDate, listRule);
                }
                else
                {
                    payslip = new Payslip(payslipDto.EmployeeId,
                        paysheetId,
                        payslipDto.MainSalary,
                        payslipDto.Allowance,
                        payslipDto.Deduction,
                        payslipDto.CommissionSalary,
                        payslipDto.OvertimeSalary,
                        payslipDto.Bonus,
                        (byte)PayslipStatuses.TemporarySalary,
                        payslipDto.Code,
                        payslipDto.Note,
                        payslipCreatedBy, payslipCreatedDate, listRule);
                }

                payslip.PayslipPenalizes = payslipDto.PayslipPenalizes?.Select(p =>
                        new PayslipPenalize(p.PayslipId, p.PenalizeId, p.Value, p.TimesCount, p.MoneyType, p.IsActive))
                    .ToList();

                payslip.PayslipClockingPenalizes = payslipDto.PayslipClockingPenalizes?.Select(p =>
                        new PayslipClockingPenalize(p.PayslipId, p.ClockingId, p.PenalizeId, p.Value, p.TimesCount, p.MoneyType, p.ClockingPenalizeCreated, p.ShiftId))
                    .ToList();

                newPayslips.Add(payslip);
            }

            return newPayslips;
        }

        private async Task MakePayslipPaymentsForPaysheetAsync(PaysheetDto paysheetDto, Paysheet existingPaysheet, bool isComplete = false, bool isCreate = false)
        {
            if (paysheetDto.Payslips != null && paysheetDto.Payslips.Any())
            {
                paysheetDto.Payslips.ForEach(ps =>
                {
                    var createdPayslip = existingPaysheet.Payslips.FirstOrDefault(p => p.EmployeeId == ps.EmployeeId);
                    if (createdPayslip == null) return;
                    ps.Id = createdPayslip.Id;
                    ps.Code = createdPayslip.Code;
                    ps.PaysheetId = existingPaysheet.Id;
                    ps.PayslipCreatedDate = createdPayslip.PayslipCreatedDate;
                });
                if (isCreate)
                    paysheetDto.Code = existingPaysheet.Code;

                await _kiotVietServiceClient.MakePayslipPaymentsForPaysheetAsync(new MakePayslipPaymentsForPaysheetReq
                {
                    Paysheet = paysheetDto,
                    IsComplete = isComplete
                });
            }
        }

        private async Task CancelPayslipPayment(List<Payslip> payslipsNeedCancel, bool isCancelPayment, bool cancelPaysheet = false)
        {
            if (payslipsNeedCancel != null && payslipsNeedCancel.Any())
            {
                await _kiotVietServiceClient.VoidPayslipPaymentsAsync(
                    new VoidPayslipPaymentReq
                    {
                        Payslips = _mapper.Map<List<PayslipDto>>(payslipsNeedCancel),
                        IsVoidPayslipPayment = isCancelPayment,
                        IsCancelPaysheet = cancelPaysheet
                    });
            }
        }

        private async Task<ValidationResult> ValidatePaysheetAndPayslip(Paysheet existingPaysheet, bool isChangeCode, bool isCreate)
        {
            // Lấy thông tin phiếu thanh toán để validate với thời gian lập bảng lương
            var payslipPayments =
                await _kiotVietServiceClient.GetPayslipPaymentsAsync(
                    new GetPayslipPaymentsReq
                    {
                        PaysheetId = existingPaysheet.Id
                    });

            var firstPayslipPayment = payslipPayments?.Data?.Where(p => p.Status != (byte)PayslipPaymentStatuses.Void)
                .OrderBy(p => p.TransDate).FirstOrDefault();

            // Paysheet validator
            var validatorResult =
                await (new CreateOrUpdatePaysheetValidator(_paysheetWriteOnlyRepository, existingPaysheet.Payslips, firstPayslipPayment,
                    isChangeCode, isCreate).ValidateAsync(existingPaysheet));
            if (!validatorResult.IsValid)
            {
                return validatorResult;
            }

            // Payslip validator
            validatorResult =
                await (new CreateOrUpdatePayslipValidator(_payslipReadOnlyRepository, _employeeReadOnlyRepository)
                    .ValidateAsync(existingPaysheet.Payslips));
            return validatorResult;
        }

        private bool CheckDuplicateDeductionName(List<PayslipDto> payslipDtos)
        {
            if (payslipDtos != null && payslipDtos.Any())
            {
                return false;
            }

            var payslips = payslipDtos.Where(x => x.DeductionRuleParam.Deductions.Count > 0);
            foreach (var payslip in payslips)
            {
                var duplicateDeductionNames = payslip.DeductionRuleParam?.Deductions?.Where(p => p.Name != null).GroupBy(p => p.Name.ToLower()).Where(g => g.Count() > 1).Select(g => g.Key);
                if (duplicateDeductionNames?.Any() == true)
                {
                    return true;
                }
            }

            return false;
        }

        private void NotifyPaysheetInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Paysheet).Name, string.Format(Message.not_exists, Label.paysheet)));
        }

        private void NotifyDedcutionNameInDbIsExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Paysheet).Name, string.Format(Message.is_exists, Label.deduction_name)));
        }
    }
}
