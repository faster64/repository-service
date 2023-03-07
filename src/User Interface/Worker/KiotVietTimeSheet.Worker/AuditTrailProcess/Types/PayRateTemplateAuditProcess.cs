using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.PayRateTemplateEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using ServiceStack;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class PayRateTemplateAuditProcess : BaseAuditProcess
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly IMapper _mapper;
        public PayRateTemplateAuditProcess(
            IKiotVietInternalService kiotVietInternalService,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IMapper mapper
        ) : base(kiotVietInternalService)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task WriteCreatePayRateTemplateLogAsync(CreatedPayRateTemplateIntegrationEvent @event)
        {
            var payRateTemplateDto = _mapper.Map<PayRateFormDto>(@event.PayRateTemplate);
            TimeSheetFunctionTypes functionType = @event.IsGeneralSetting ? TimeSheetFunctionTypes.GeneralSettings : TimeSheetFunctionTypes.EmployeeManagement;
            var auditLog = GenerateLog(
                        functionType,
                        TimeSheetAuditTrailAction.Create,
                        $"Thêm mới mẫu lương: {payRateTemplateDto.Name}</br>" +
                        $"- Kỳ hạn trả lương: {((PaySheetWorkingPeriodStatuses)@event.PayRateTemplate.SalaryPeriod).ToDescription()}</br>" +
                        $"{GetNameAuditString(await RenderCreateCommissionLogs(payRateTemplateDto))}" +
                        $"{GetNameAuditString(RenderCreateAllowanceLogs(payRateTemplateDto))}" +
                        $"{GetNameAuditString(RenderCreateDeductionLogs(payRateTemplateDto))}",
                        @event.Context
                    );
            await AddLogAsync(auditLog);
        }

        public async Task WriteUpdatePayRateTemplateLogAsync(UpdatedPayRateTemplateIntegrationEvent @event)
        {
            var oldPayRateTemplateDto = _mapper.Map<PayRateFormDto>(@event.OldPayRateTemplate);
            var newPayRateTemplateDto = _mapper.Map<PayRateFormDto>(@event.NewPayRateTemplate);
            TimeSheetFunctionTypes functionType = @event.IsGeneralSetting ? TimeSheetFunctionTypes.GeneralSettings : TimeSheetFunctionTypes.EmployeeManagement;
            var content = "Cập nhật mẫu lương: " +
                          $"{RenderPayRateTemplateName(oldPayRateTemplateDto.Name, newPayRateTemplateDto.Name)} </br>" +
                          $"{GetNameAuditString(RenderUpdateSalaryPeriodLogs(oldPayRateTemplateDto, newPayRateTemplateDto))}" +
                          $"{GetNameAuditString(await RenderUpdateCommissionLogs(oldPayRateTemplateDto, newPayRateTemplateDto))}" +
                          $"{GetNameAuditString(RenderUpdateAllowanceLogs(oldPayRateTemplateDto, newPayRateTemplateDto))}" +
                          $"{GetNameAuditString(RenderUpdateDeductionLogs(oldPayRateTemplateDto, newPayRateTemplateDto))}";
            var auditLog = GenerateLog(
                functionType,
                TimeSheetAuditTrailAction.Update,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteDeletePayRateTemplateLogAsync(DeletedPayRateTemplateIntegrationEvent @event)
        {
            TimeSheetFunctionTypes functionType = @event.IsGeneralSetting ? TimeSheetFunctionTypes.GeneralSettings : TimeSheetFunctionTypes.EmployeeManagement;
            var auditLog = GenerateLog(
                        functionType,
                        TimeSheetAuditTrailAction.Delete,
                        $"Xóa mẫu áp dụng: {@event.PayRateTemplate.Name}",
                        @event.Context
                    );
            await AddLogAsync(auditLog);
        }

        private static string RenderPayRateTemplateName(string oldPayRateTemplateName, string newPayRateTemplateName)
        {
            return oldPayRateTemplateName == newPayRateTemplateName ? $"{oldPayRateTemplateName}" : $"{oldPayRateTemplateName} -> {newPayRateTemplateName}";
        }
        private string GetNameAuditString(string message)
        {
            return (string.IsNullOrEmpty(message) ? string.Empty : message);
        }

        private async Task<string> RenderCreateCommissionLogs(PayRateFormDto payRateTemplateDto)
        {
            var template = string.Empty;
            var commission = payRateTemplateDto.CommissionSalaryRuleValue;

            if (commission == null)
            {
                return template;
            }

            template += $"- Hoa hồng: {commission.FormalityTypes.ToDescription()}</br>";
            template += $"  + Hình thức: {commission.Type.ToDescription()}</br>";

            if (commission.CommissionSalaryRuleValueDetails == null || commission.CommissionSalaryRuleValueDetails.Count == 0)
            {
                return template;
            }
            var commissionTableLst = await _commissionReadOnlyRepository.GetAllCommission();
            foreach (var item in commission.CommissionSalaryRuleValueDetails)
            {
                if (item.ValueRatio.HasValue)
                {
                    template += $"  + Từ {item.CommissionLevel} - {item.ValueRatio}% doanh thu vượt</br>";
                }
                if (item.Value.HasValue)
                {
                    template += $"  + Từ {item.CommissionLevel} - {item.Value} VND</br>";
                }
                if (item.CommissionTableId.HasValue)
                {
                    var commissionTableName = commissionTableLst.FirstOrDefault(x => x.Id == item.CommissionTableId)?.Name;
                    template += $"  + Từ {item.CommissionLevel} - {commissionTableName}</br>";
                }
            }

            return template;
        }

        private string RenderCreateAllowanceLogs(PayRateFormDto payRateTemplateDto)
        {
            var template = string.Empty;
            var allowance = payRateTemplateDto.AllowanceRuleValue;

            if (allowance == null || allowance.AllowanceRuleValueDetails == null || allowance.AllowanceRuleValueDetails.Count == 0)
            {
                return template;
            }

            var allowanceLst = _allowanceReadOnlyRepository.GetAllByTenant();
            template += $"- Phụ cấp:</br>";
            foreach (var item in allowance.AllowanceRuleValueDetails)
            {
                var allowanceName = allowanceLst.FirstOrDefault(x => x.Id == item.AllowanceId)?.Name;
                if (item.ValueRatio.HasValue)
                {
                    template += $"  + {allowanceName}: {item.ValueRatio}% lương chính</br>";
                }
                if (item.Value.HasValue)
                {
                    template += $"  + {allowanceName}: {item.Value} VND - {item.Type.ToDescription()}</br>";
                }
            }

            return template;
        }

        private string RenderCreateDeductionLogs(PayRateFormDto payRateTemplateDto)
        {
            var template = string.Empty;
            var deduction = payRateTemplateDto.DeductionRuleValue;

            if (deduction == null || deduction.DeductionRuleValueDetails == null || deduction.DeductionRuleValueDetails.Count == 0)
            {
                return template;
            }

            var deductionLst = _deductionReadOnlyRepository.GetAllByTenant();
            template += $"- Giảm trừ:</br>";
            foreach (var item in deduction.DeductionRuleValueDetails)
            {
                var deductionName = deductionLst.FirstOrDefault(x => x.Id == item.DeductionId)?.Name;
                if (item.ValueRatio.HasValue)
                {
                    template += $"  + {deductionName}: {item.ValueRatio}% Tổng thu nhập</br>";
                }
                if (item.Value.HasValue)
                {
                    template += $"  + {deductionName}: {RenderDeductionValue(item)} - {((DeductionTypes)item.DeductionRuleId).ToDescription()}</br>";
                }
            }

            return template;
        }

        private string RenderDeductionValue(DeductionRuleValueDetail deductionRuleValueDetail)
        {
            if (deductionRuleValueDetail.DeductionRuleId == (int)DeductionTypes.Fixed)
            {
                return $"{deductionRuleValueDetail.Value} VND";
            }
            if (deductionRuleValueDetail.DeductionTypeId == (int)DeductionRuleTypes.Minute)
            {
                return $"Block = {deductionRuleValueDetail.Value} VND/{deductionRuleValueDetail.BlockTypeMinuteValue} phút";
            }
            else if (deductionRuleValueDetail.DeductionTypeId == (int)DeductionRuleTypes.Time)
            {
                return $"{deductionRuleValueDetail.Value} VND/ lần";
            }
            else
            {
                return string.Empty;
            }
        }

        private string RenderUpdateSalaryPeriodLogs(PayRateFormDto oldPayRateTemplate, PayRateFormDto newPayRateTemplate)
        {
            if (oldPayRateTemplate.SalaryPeriod != newPayRateTemplate.SalaryPeriod)
            {
                var oldSalaryPeriodName = ((PaySheetWorkingPeriodStatuses)oldPayRateTemplate.SalaryPeriod).ToDescription();
                var newSalaryPeriodName = ((PaySheetWorkingPeriodStatuses)newPayRateTemplate.SalaryPeriod).ToDescription();
                return $"- Kỳ hạn trả lương: {oldSalaryPeriodName} -> {newSalaryPeriodName}</br>";
            }
            else
            {
                return string.Empty;
            }
        }

        private async Task<string> RenderUpdateCommissionLogs(PayRateFormDto oldPayRateTemplate, PayRateFormDto newPayRateTemplate)
        {
            var template = string.Empty;
            var oldCommission = oldPayRateTemplate.CommissionSalaryRuleValue;
            var newCommission = newPayRateTemplate.CommissionSalaryRuleValue;

            if (newCommission == null)
            {
                return template;
            }

            template += RenderCommissionType(oldPayRateTemplate, newPayRateTemplate);

            if (newCommission.CommissionSalaryRuleValueDetails == null || newCommission.CommissionSalaryRuleValueDetails.Count == 0)
            {
                return template;
            }
            var commissionTableLst = await _commissionReadOnlyRepository.GetAllCommission();
            var oldCommissionLevelList = oldCommission.CommissionSalaryRuleValueDetails.Select(x => x.CommissionLevel.Value).ToList();
            var newCommissionLevelList = newCommission.CommissionSalaryRuleValueDetails.Select(x => x.CommissionLevel.Value).ToList();
            var needAddCommissionLevelList = newCommissionLevelList.Except(oldCommissionLevelList).ToList();
            var needAddCommissionList = newCommission.CommissionSalaryRuleValueDetails
                                        .Where(x => needAddCommissionLevelList.Contains(x.CommissionLevel.Value))
                                        .ToList();
            foreach (var item in needAddCommissionList)
            {
                if (item.ValueRatio.HasValue)
                {
                    template += $"  + Từ {item.CommissionLevel} - {item.ValueRatio}% doanh thu vượt</br>";
                }
                if (item.Value.HasValue)
                {
                    template += $"  + Từ {item.CommissionLevel} - {item.Value} VND</br>";
                }
                if (item.CommissionTableId.HasValue)
                {
                    var commissionTableName = commissionTableLst.FirstOrDefault(x => x.Id == item.CommissionTableId)?.Name;
                    template += $"  + Từ {item.CommissionLevel} - {commissionTableName}</br>";
                }
            }

            return template;
        }
        private string RenderCommissionType(PayRateFormDto oldPayRateTemplate, PayRateFormDto newPayRateTemplate)
        {
            var template = string.Empty;
            var oldCommission = oldPayRateTemplate.CommissionSalaryRuleValue;
            var newCommission = newPayRateTemplate.CommissionSalaryRuleValue;

            if (oldCommission == null)
            {
                template += $"- Hoa hồng: {newCommission.FormalityTypes.ToDescription()}</br>";
                template += $"  + Hình thức: {newCommission.Type.ToDescription()}</br>";
            }
            else
            {
                var isChangeCommissionType = oldCommission.FormalityTypes != newCommission.FormalityTypes || oldCommission.Type != newCommission.Type;
                var oldCommissionList = oldCommission.CommissionSalaryRuleValueDetails.Select(x => x.CommissionLevel.Value).ToList();
                var newCommissionList = newCommission.CommissionSalaryRuleValueDetails.Select(x => x.CommissionLevel.Value).ToList();
                var isChangeCommissionList = newCommissionList.Except(oldCommissionList).Any();
                if (!isChangeCommissionType && !isChangeCommissionList)
                {
                    return template;
                }
                else
                {
                    if (oldCommission.FormalityTypes == newCommission.FormalityTypes)
                    {
                        template += $"- Hoa hồng: {oldCommission.FormalityTypes.ToDescription()}</br>";
                    }
                    else
                    {
                        template += $"- Hoa hồng: {oldCommission.FormalityTypes.ToDescription()} -> {newCommission.FormalityTypes.ToDescription()}</br>";
                    }
                    if (oldCommission.Type == newCommission.Type)
                    {
                        template += $"  + Hình thức: {oldCommission.Type.ToDescription()}</br>";
                    }
                    else
                    {
                        template += $"  + Hình thức: {oldCommission.Type.ToDescription()} -> {newCommission.Type.ToDescription()}</br>";
                    }
                }
            }

            return template;
        }

        private string RenderUpdateAllowanceLogs(PayRateFormDto oldPayRateTemplate, PayRateFormDto newPayRateTemplate)
        {
            var template = string.Empty;
            var oldAllowance = oldPayRateTemplate.AllowanceRuleValue;
            var newAllowance = newPayRateTemplate.AllowanceRuleValue;

            if (newAllowance == null || newAllowance.AllowanceRuleValueDetails == null || newAllowance.AllowanceRuleValueDetails.Count == 0)
            {
                return template;
            }

            var allowanceLst = _allowanceReadOnlyRepository.GetAllByTenant();
            var oldAllowanceIdList = oldAllowance.AllowanceRuleValueDetails.Select(x => x.AllowanceId).ToList();
            var newAllowanceIdList = newAllowance.AllowanceRuleValueDetails.Select(x => x.AllowanceId).ToList();
            var needAddAllowanceIdList = newAllowanceIdList.Except(oldAllowanceIdList).ToList();
            var needAddAllowanceList = newAllowance.AllowanceRuleValueDetails
                                        .Where(x => needAddAllowanceIdList.Contains(x.AllowanceId))
                                        .ToList();
            if (needAddAllowanceList.Count > 0)
            {
                template += $"- Phụ cấp:</br>";
            }

            foreach (var item in needAddAllowanceList)
            {
                var allowanceName = allowanceLst.FirstOrDefault(x => x.Id == item.AllowanceId)?.Name;
                if (item.ValueRatio.HasValue)
                {
                    template += $"  + {allowanceName}: {item.ValueRatio}% lương chính</br>";
                }
                if (item.Value.HasValue)
                {
                    template += $"  + {allowanceName}: {item.Value} VND - {item.Type.ToDescription()}</br>";
                }
            }

            return template;
        }

        private string RenderUpdateDeductionLogs(PayRateFormDto oldPayRateTemplate, PayRateFormDto newPayRateTemplate)
        {
            var template = string.Empty;
            var oldDeduction = oldPayRateTemplate.DeductionRuleValue;
            var newDeduction = newPayRateTemplate.DeductionRuleValue;

            if (newDeduction == null || newDeduction.DeductionRuleValueDetails == null || newDeduction.DeductionRuleValueDetails.Count == 0)
            {
                return template;
            }

            var deductionLst = _deductionReadOnlyRepository.GetAllByTenant();
            var oldDeductionIdList = oldDeduction.DeductionRuleValueDetails.Select(x => x.DeductionId).ToList();
            var newDeductionIdList = newDeduction.DeductionRuleValueDetails.Select(x => x.DeductionId).ToList();
            var needAddDeductionIdList = newDeductionIdList.Except(oldDeductionIdList).ToList();
            var needAddDeductionList = newDeduction.DeductionRuleValueDetails
                                        .Where(x => needAddDeductionIdList.Contains(x.DeductionId))
                                        .ToList();
            if (needAddDeductionList.Count > 0)
            {
                template += $"- Giảm trừ:</br>";
            }

            foreach (var item in needAddDeductionList)
            {
                var deductionName = deductionLst.FirstOrDefault(x => x.Id == item.DeductionId)?.Name;
                if (item.ValueRatio.HasValue)
                {
                    template += $"  + {deductionName}: {item.ValueRatio}% Tổng thu nhập</br>";
                }
                if (item.Value.HasValue)
                {
                    template += $"  + {deductionName}: {RenderDeductionValue(item)} - {((DeductionTypes)item.DeductionRuleId).ToDescription()}</br>";
                }
            }

            return template;
        }

    }
}
