using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using ServiceStack;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types
{
    public class PaysheetAuditProcess : BaseAuditProcess
    {
        private readonly ICacheClient _cacheClient;
        private readonly IKiotVietApiClient _kiotVietApiClient;
        private readonly IAppSettings _appSettings;
        private readonly Helper _helper = new Helper();
        private EfDbContext _db;
        public PaysheetAuditProcess(
            IKiotVietApiClient kiotVietApiClient,
            IAuditProcessFailEventService auditProcessFailEventService, 
            IAppSettings appSettings, 
            ICacheClient cacheClient
            ) : base(kiotVietApiClient, auditProcessFailEventService)
        {
            _cacheClient = cacheClient;
            _kiotVietApiClient = kiotVietApiClient;
            _appSettings = appSettings;
        }

        public async Task WriteCreatePaysheetLogAsync(CreatedPaysheetIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                        @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var content = "Thêm mới bảng lương:<br>" +
                                      $"- Mã BL: [PaysheetCode]{@event.Paysheet.Code}[/PaysheetCode], " +
                                      await RenderPaysheetInfo(@event.Paysheet, @event.Context.GroupId, @event.Context.RetailerCode);
                        var auditLog = GenerateLog(
                            TimeSheetFunctionTypes.PaysheetManagement,
                            TimeSheetAuditTrailAction.Create,
                            content,
                            @event.Context
                        );
                        await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                    }
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteUpdatePaysheetLogAsync(UpdatedPaysheetIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                        @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var log = "Cập nhật thông tin bảng lương:<br>" +
                                  $"- Mã BL: {RenderPaysheetCode(@event.OldPaysheet.Code, @event.NewPaysheet.Code)}" +
                                  await RenderPaysheetInfo(@event.NewPaysheet, @event.Context.GroupId, @event.Context.RetailerCode);
                        var auditLog = GenerateLog(
                            TimeSheetFunctionTypes.PaysheetManagement,
                            TimeSheetAuditTrailAction.Update,
                            log,
                            @event.Context
                        );
                        await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                    }
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteCancelPaysheetLogAsync(CancelPaysheetIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var log = $"Hủy bảng lương: Mã: {@event.Paysheet.Code}, Tên: {@event.Paysheet.Name}, Trạng thái: {RenderPaysheetStatusText(@event.Paysheet.PaysheetStatus)}";
                    var auditLog = GenerateLog(
                        TimeSheetFunctionTypes.PaysheetManagement,
                        TimeSheetAuditTrailAction.Reject,
                        log,
                        @event.Context
                    );
                    await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        #region Private Methods

        private async Task<string> RenderPayslip(List<Payslip> payslips)
        {
            var content = string.Empty;
            if (payslips == null || !payslips.Any())
            {
                return content;
            }

            var employeeIds = payslips.Select(payslip => payslip.EmployeeId).ToList();
            var employees = await _db.Employees.Where(employee => employeeIds.Contains(employee.Id)).AsNoTracking().ToListAsync();

            var payslipDetailLogs = payslips.Select(payslip =>
            {
                var employee = employees.FirstOrDefault(e => e.Id == payslip.EmployeeId);
                var employeeCode = employee?.Code ?? string.Empty;
                var employeeName = employee?.Name ?? string.Empty;

                var payslipDetails = payslip.PayslipDetails;
                var payslipRules = payslipDetails.Select(x => RuleFactory.GetRule(x.RuleType, x.RuleValue)).ToList();
                var existMainRule = payslipRules.FirstOrDefault(x => x.GetType() == typeof(MainSalaryRule));
                var existCommissionRule = payslipRules.FirstOrDefault(x => x.GetType() == typeof(CommissionSalaryRuleV2));
                var existOvertimeRule = payslipRules.FirstOrDefault(x => x.GetType() == typeof(OvertimeSalaryRule));
                var existAllowanceRule = payslipRules.FirstOrDefault(x => x.GetType() == typeof(AllowanceRule));
                var existDeductionRule = payslipRules.FirstOrDefault(x => x.GetType() == typeof(DeductionRule));

                const string notApplyText = "Không áp dụng";
                var mainSalaryText = existMainRule == null ? $"Lương chính: {notApplyText}, " : $"Lương chính: {payslip.MainSalary:n0}, ";
                var commissionSalaryText = existCommissionRule == null ? $"Hoa hồng: {notApplyText}, " : $"Hoa hồng: {payslip.CommissionSalary:n0}, ";
                var overtimeSalaryText = existOvertimeRule == null ? $"Lương làm thêm: {notApplyText}, " : $"Lương làm thêm: {payslip.OvertimeSalary:n0}, ";
                var allowanceSalaryText = existAllowanceRule == null ? $"Phụ cấp: {notApplyText}, " : $"Phụ cấp: {payslip.Allowance:n0}, ";
                var deductionSalaryText = existDeductionRule == null ? $"Giảm trừ: {notApplyText}, " : $"Giảm trừ: {payslip.Deduction:n0}, ";


                return $"</br>- Mã nhân viên: [EmployeeCode]{employeeCode}[/EmployeeCode], " +
                           $"Tên NV: {employeeName}, " +
                           $"{mainSalaryText}" +
                           $"{commissionSalaryText}" +
                           $"{overtimeSalaryText} " +
                           $"{allowanceSalaryText}" +
                           $"Thưởng: {payslip.Bonus:n0}, " +
                           $"{deductionSalaryText}" +
                           $"Tổng lương: {payslip.NetSalary:n0}";
            }).Select(x => x);

            content = string.Join("", payslipDetailLogs);
            return content;
        }

        private static string RenderPaysheetCode(string oldPaysheetString, string newPaysheetString)
        {
            if (oldPaysheetString == newPaysheetString)
            {
                return $"[PaysheetCode]{newPaysheetString}[/PaysheetCode], ";
            }

            return $"{oldPaysheetString} -> [PaysheetCode]{newPaysheetString}[/PaysheetCode], ";
        }

        private static string RenderPaysheetStatusText(byte paysheetStatus)
        {
            switch (paysheetStatus)
            {
                case (byte)PaysheetStatuses.TemporarySalary:
                    return PaysheetStatuses.TemporarySalary.ToDescription();
                case (byte)PaysheetStatuses.PaidSalary:
                    return PaysheetStatuses.PaidSalary.ToDescription();
                default:
                    return PaysheetStatuses.Void.ToDescription();
            }
        }

        private static decimal RenderMainSalary(Paysheet paysheet)
        {
            var mainSalary = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted && !payslip.IsDraft && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.MainSalary);
            return mainSalary;
        }

        private static decimal RenderCommissionSalary(Paysheet paysheet)
        {
            var commissionSalary = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted && !payslip.IsDraft && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.CommissionSalary);
            return commissionSalary;
        }

        private static decimal RenderOvertimeSalary(Paysheet paysheet)
        {
            var overtimeSalary = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted && !payslip.IsDraft && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.OvertimeSalary);
            return overtimeSalary;
        }

        private static decimal RenderAllowance(Paysheet paysheet)
        {
            var allowance = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted && !payslip.IsDraft && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.Allowance);
            return allowance;
        }

        private static decimal RenderBonus(Paysheet paysheet)
        {
            var bonus = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted && !payslip.IsDraft && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.Bonus);
            return bonus;
        }

        private static decimal RenderDeduction(Paysheet paysheet)
        {
            var deduction = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted && !payslip.IsDraft && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.Deduction);
            return deduction;
        }

        private static decimal RenderNetSalary(Paysheet paysheet)
        {
            var netSalary = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted && !payslip.IsDraft && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.NetSalary);
            return netSalary;
        }

        private async Task<string> RenderPaysheetInfo(Paysheet paysheet, int groupId, string retailerCode = "")
        {
            var branch = await KiotVietApiClient.GetBranchById(paysheet.BranchId, paysheet.TenantId, groupId, retailerCode);
            var branchName = branch?.Name ?? string.Empty;

            var payslips = paysheet.Payslips.Where(x =>
                !x.IsDeleted && x.PayslipStatus != (byte)PayslipStatuses.Void &&
                x.PayslipStatus != (byte)PayslipStatuses.Draft).ToList();

            return $"Tên BL: {paysheet.Name}, " +
                   $"Chi nhánh: {branchName}, " +
                   $"Kỳ làm việc: {paysheet.StartTime:dd/MM/yyyy} - {paysheet.EndTime:dd/MM/yyyy}, " +
                   $"Trạng thái: {RenderPaysheetStatusText(paysheet.PaysheetStatus)}<br>" +
                   $"- Tổng lương chính: {RenderMainSalary(paysheet):n0}, " +
                   $"Tổng hoa hồng: {RenderCommissionSalary(paysheet):n0}, " +
                   $"Tổng lương làm thêm: {RenderOvertimeSalary(paysheet):n0}, " +
                   $"Tổng phụ cấp: {RenderAllowance(paysheet):n0}, " +
                   $"Tổng thưởng: {RenderBonus(paysheet):n0}, " +
                   $"Tổng giảm trừ: {RenderDeduction(paysheet):n0}, " +
                   $"Tổng lương: {RenderNetSalary(paysheet):n0}<br>" +
                   $"Gồm các phiếu lương: {RenderPayslip(payslips).Result}";
        }
        #endregion
    }
}
