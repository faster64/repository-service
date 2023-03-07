using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Application.Utilities;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.ExportEmployeePayslipData
{
    public class ExportEmployeePayslipCommandHandler : BaseCommandHandler,
        IRequestHandler<ExportEmployeePayslipCommand, PayslipDto>
    {
        #region Properties
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;

        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly IAuthService _authService;
        private readonly IPenalizeReadOnlyRepository _penalizeReadOnlyRepository;
        private readonly IMediator _mediator;

        #endregion

        public ExportEmployeePayslipCommandHandler(
            IAuthService authService,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IMapper mapper,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IEventDispatcher eventDispatcher,
            IPenalizeReadOnlyRepository penalizeReadOnlyRepository,
            IMediator mediator
        ) : base(eventDispatcher)
        {
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _mapper = mapper;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
            _authService = authService;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _penalizeReadOnlyRepository = penalizeReadOnlyRepository;
            _mediator = mediator;
        }

        public async Task<PayslipDto> Handle(ExportEmployeePayslipCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _authService.Context.TenantId;

            var paySheet = await _paysheetReadOnlyRepository.FindBySpecificationAsync(
                new FindPaysheetByIdSpec(request.PaySheetId)
                    .And(new FindPaysheetByBranchId(request.BranchId))
                    .And(new FindPaysheetByTenantIdSpec(tenantId))
                );

            if (paySheet == null)
            {
                return new PayslipDto();
            }

            // Lấy thời gian ngày công chuẩn trong thiết lập cửa hàng
            var paySheetDto = _mapper.Map<PaysheetDto>(paySheet);
            paySheetDto.Code = paySheet.PaysheetStatus == (byte)PaysheetStatuses.Draft ? string.Empty : paySheet.Code;

            var payslipSpec = new GetPayslipByPaysheetId(request.PaySheetId)
                .And(new FindPayslipByIdSpec(request.PayslipId))
                .And(new FindPayslipByEmployeeId(request.EmployeeId));

            var payslips = await _payslipReadOnlyRepository.GetBySpecificationAsync(payslipSpec, true);

            var employee = await _employeeReadOnlyRepository.FindAndIncludeBySpecificationAsync(new FindEmployeeByIdSpec(request.EmployeeId), new string[] { nameof(EmployeeDto.JobTitle), nameof(EmployeeDto.Department) }, true);
            var employeeDtoItem = _mapper.Map<EmployeeDto>(employee);

            var payslip = payslips.FirstOrDefault();
            if (payslip == null) return new PayslipDto();

            var payslipDtoItem = _mapper.Map<PayslipDto>(payslip);

            var settingObjectDto = await _mediator.Send(new GetSettingQuery(tenantId), cancellationToken);

            var allowanceIds = payslipDtoItem.AllowanceRuleParam?.Allowances?.Select(x => x.AllowanceId).ToList();
            if (allowanceIds != null && allowanceIds.Any())
            {
                var allowanceFromSource =
                    await _allowanceReadOnlyRepository.GetBySpecificationAsync(
                        new FindAllowanceByIdsSpec(allowanceIds).And(new FindAllowanceByTenantIdSpec(tenantId)));

                payslipDtoItem.AllowanceRuleParam?.Allowances?.ForEach(allowance =>
                {
                    allowance.Name = allowanceFromSource.Where(x => x.Id == allowance.AllowanceId).Select(x => x.Name).FirstOrDefault() ?? allowance.Name;
                });
            }

            if (payslipDtoItem.AllowanceRuleValue?.AllowanceRuleValueDetails != null)
            {
                payslipDtoItem.AllowanceRuleValue.AllowanceRuleValueDetails = payslipDtoItem.AllowanceRuleValue.AllowanceRuleValueDetails.Where(x => allowanceIds.Contains(x.AllowanceId)).ToList();
            }

            if (payslipDtoItem?.DeductionRuleParam?.Deductions != null)
                payslipDtoItem = await SetOtherDeduction(payslipDtoItem, tenantId);

            if (payslipDtoItem.PayslipPenalizes != null)
            {
                var payslipPenalizes = payslipDtoItem.PayslipPenalizes.Where(p => p.IsActive).ToList();
                payslipDtoItem = await MergePanelizeToDeduction(payslipDtoItem, payslipPenalizes);
            }

            var payslipPaymentPaid = await _kiotVietServiceClient.GetPayslipPaymentPaidPayslipAsync(new GetPaidPayslipPaymentReq()
            {
                EmployeeId = request.EmployeeId,
                PayslipId = request.PayslipId,
                IncludeUnAllocate = true, //Lấy phiếu thanh toán của phiếu đã chốt lương
                Includes = new string[] { "CreatedName", "EmployeeCode" }
            });

            if (payslipPaymentPaid.Data != null && payslipPaymentPaid.Data.Any())
            {
                payslipDtoItem.PayslipPayments = payslipPaymentPaid.Data.ToList();
            }

            #region Tính ngày nghỉ có phép
            payslipDtoItem.AuthorisedAbsence = HaftShiftUtils.CalculateAuthorisedAbsence(payslipDtoItem, settingObjectDto);
            #endregion

            #region Tính ngày nghỉ không phép
            payslipDtoItem.UnauthorisedAbsence = 0;

            var clockingUnAuthorizeAbsences = await _clockingReadOnlyRepository.GetClockingUnAuthorizeAbsenceByPaySheetIds(
                new List<long>() { request.PaySheetId }, new List<long>() { request.PayslipId }, request.EmployeeId);
            var isClockingUnAuthorizeAbsence = clockingUnAuthorizeAbsences != null && clockingUnAuthorizeAbsences.Any();

            if (isClockingUnAuthorizeAbsence)
            {
                var clockingUnAuthorizeAbsenceKeyValuePairs = clockingUnAuthorizeAbsences.FirstOrDefault(c => c.Key == request.PayslipId);
                payslipDtoItem.UnauthorisedAbsence = clockingUnAuthorizeAbsenceKeyValuePairs.Value;
            }
            #endregion

            payslipDtoItem.Employee = employeeDtoItem;
            payslipDtoItem.Code = payslipDtoItem.PayslipStatus == (byte)PayslipStatuses.Draft ? string.Empty : payslipDtoItem.Code;
            payslipDtoItem.PayslipClockings = _mapper.Map<List<PayslipClockingDto>>(payslip.PayslipClockings);
            payslipDtoItem.Paysheet = paySheetDto;

            return payslipDtoItem;
        }

        private async Task<PayslipDto> MergePanelizeToDeduction(PayslipDto payslipDtoItem, IList<PayslipPenalizeDto> payslipPenalizes)
        {
            var maxDeductionId = (long)0;
            var penalizeIds = payslipPenalizes.Select(p => p.PenalizeId).ToList();
            object[] penalizeIdsArr = { penalizeIds };
            var penalizes = await _penalizeReadOnlyRepository.FindByIdsAsync(penalizeIdsArr, false, true);
            var penalizeDtos = _mapper.Map<List<PenalizeDto>>(penalizes);

            var deductionRuleParam = new DeductionRuleParam();
            if (payslipDtoItem.DeductionRuleParam == null) payslipDtoItem.DeductionRuleParam = deductionRuleParam;
            var deductionRuleValue = new DeductionRuleValue();
            if (payslipDtoItem.DeductionRuleValue == null) payslipDtoItem.DeductionRuleValue = deductionRuleValue;

            var payslipPenalizeDeductions = new List<DeductionParam>();
            var payslipPenalizeDeductionValues = new List<DeductionRuleValueDetail>();

            if (payslipDtoItem.DeductionRuleParam?.Deductions != null && (payslipDtoItem.DeductionRuleParam?.Deductions).Any())
                maxDeductionId = payslipDtoItem.DeductionRuleParam.Deductions.Max(p => p.DeductionId);

            foreach (var payslipPenalize in payslipPenalizes)
            {
                maxDeductionId++;
                if (payslipDtoItem.Id != payslipPenalize.PayslipId) continue;
                var payslipPenalizeDeductionParam = new DeductionParam
                {
                    Value = payslipPenalize.Value,
                    Name = $"{penalizeDtos.FirstOrDefault(p => p.Id == payslipPenalize.PenalizeId)?.Name} ({payslipPenalize.TimesCount} lần)",
                    DeductionId = maxDeductionId
                };

                var payslipPenalizeDeductionValue = new DeductionRuleValueDetail
                {
                    Value = payslipPenalize.Value,
                    Name = penalizeDtos.FirstOrDefault(p => p.Id == payslipPenalize.PenalizeId)?.Name,
                    DeductionId = maxDeductionId
                };
                payslipPenalizeDeductions.Add(payslipPenalizeDeductionParam);
                payslipPenalizeDeductionValues.Add(payslipPenalizeDeductionValue);
            }

            if (payslipDtoItem.DeductionRuleParam.Deductions == null)
                payslipDtoItem.DeductionRuleParam.Deductions = payslipPenalizeDeductions;

            payslipDtoItem.DeductionRuleParam.Deductions.AddRange(payslipPenalizeDeductions);
            payslipDtoItem.DeductionRuleValue.DeductionRuleValueDetails.AddRange(payslipPenalizeDeductionValues);
            return payslipDtoItem;
        }

        private async Task<PayslipDto> SetOtherDeduction(PayslipDto payslipDtoItem, int tenantId)
        {
            var deductions = payslipDtoItem.DeductionRuleParam.Deductions.ToList();
            if (!deductions.Any()) return payslipDtoItem;

            var deductionIds = deductions.Select(x => x.DeductionId).ToList();
            if (!deductionIds.Any()) return payslipDtoItem;

            var maxDeductionId = deductions.Max(d => d.DeductionId);
            var payslipListClocking = payslipDtoItem.PayslipClockings ?? new List<PayslipClockingDto>();
            var listClockingWithLateTimeCount = payslipListClocking.Count(x => x.TimeIsLate > 0);
            if (listClockingWithLateTimeCount < 1) listClockingWithLateTimeCount = 1;

            var deductionFromSource =
                await _deductionReadOnlyRepository.GetBySpecificationAsync(
                    new FindDeductionByIdsSpec(deductionIds).And(new FindDeductionByTenantIdSpec(tenantId)), false, true);

            maxDeductionId++;

            payslipDtoItem.DeductionRuleParam.Deductions = SetDeductionParams(
                deductions,
                payslipDtoItem,
                listClockingWithLateTimeCount,
                payslipListClocking,
                deductionFromSource,
                maxDeductionId);
            return payslipDtoItem;
        }

        private int GenClockingWithLateTimeCount(int listClockingWithLateTimeCount, DeductionRuleValueDetail deductionRuleValue, List<PayslipClockingDto> payslipListClocking)
        {
            if (deductionRuleValue == null || deductionRuleValue.DeductionTypeId != (int)DeductionRuleTypes.Minute)
                return listClockingWithLateTimeCount;
            var isLate = true;
            switch (deductionRuleValue.Type)
            {
                case DeductionTypes.Late:
                    isLate = true;
                    break;
                case DeductionTypes.Early:
                    isLate = false;
                    break;
            }
            listClockingWithLateTimeCount = GetNumberBlock(payslipListClocking, deductionRuleValue.BlockTypeMinuteValue, isLate);
            return listClockingWithLateTimeCount;
        }

        private int GetNumberBlock(List<PayslipClockingDto> payslipListClocking, decimal? value, bool isLate)
        {
            int blocks = 0;
            if (!value.HasValue || payslipListClocking == null)
                return blocks;

            if (isLate)
            {
                var numberClockingLate = payslipListClocking.Where(x => x.TimeIsLate > 0).ToList();
                foreach (var payslipClocking in numberClockingLate)
                {
                    blocks += (int)(payslipClocking.TimeIsLate / value.Value);
                    if (payslipClocking.TimeIsLate % value.Value != 0)
                        blocks += 1;
                }
            }
            else
            {
                var numberClockingEarly = payslipListClocking.Where(x => x.TimeIsLeaveWorkEarly > 0).ToList();
                foreach (var payslipClocking in numberClockingEarly)
                {
                    blocks += (int)(payslipClocking.TimeIsLeaveWorkEarly / value.Value);
                    if (payslipClocking.TimeIsLeaveWorkEarly % value.Value != 0)
                        blocks += 1;
                }
            }
            return blocks;
        }

        private List<DeductionParam> SetDeductionParams(
            List<DeductionParam> deductions,
            PayslipDto payslipDtoItem,
            int listClockingWithLateTimeCount,
            List<PayslipClockingDto> payslipListClocking,
            List<Deduction> deductionFromSource,
            long maxDeductionId)
        {
            deductions.ForEach(deduction =>
            {
                var deductionDetail = new DeductionRuleValueDetail
                {
                    Value = deduction.Value,
                    ValueRatio = deduction.ValueRatio
                };
                if (deduction.DeductionId != 0)
                {
                    var deductionRuleValue =
                        payslipDtoItem.DeductionRuleValue?.DeductionRuleValueDetails?.FirstOrDefault(d =>
                            d.DeductionId == deduction.DeductionId);

                    var deductionValue = deduction.Value;
                    if (deductionRuleValue?.Type == DeductionTypes.Late)
                    {
                        listClockingWithLateTimeCount =
                            GenClockingWithLateTimeCount(listClockingWithLateTimeCount, deductionRuleValue, payslipListClocking);
                        if (listClockingWithLateTimeCount == 0) deductionValue = 0;
                        else
                            deductionValue = deduction.Value / listClockingWithLateTimeCount;
                    }

                    if (deductionRuleValue != null)
                    {
                        deductionRuleValue.Value = deductionValue;
                        deductionRuleValue.ValueRatio = deduction.ValueRatio;
                    }
                    deduction.Name = deductionFromSource.FirstOrDefault(x => x.Id == deduction.DeductionId)?.Name;
                }
                else
                {
                    deduction.DeductionId = maxDeductionId++;
                    deductionDetail.DeductionId = deduction.DeductionId;
                    payslipDtoItem.DeductionRuleValue.DeductionRuleValueDetails.Add(deductionDetail);
                }
            });

            return deductions;
        }
    }
}
