using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using ServiceStack;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
using MediatR;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByIds;
using KiotVietTimeSheet.Application.Queries.GetPaysheetById;
using KiotVietTimeSheet.Application.ServiceClients;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class PaysheetAuditProcess : BaseAuditProcess
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public PaysheetAuditProcess(
            IKiotVietInternalService kiotVietInternalService,
            IMediator mediator,
            IMapper mapper
            ) : base(kiotVietInternalService)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task WriteCreatePaysheetLogAsync(CreatedPaysheetIntegrationEvent @event)
        {
            var paysheetDto = _mapper.Map<PaysheetDto>(@event.Paysheet);
            var content = "Thêm mới bảng lương:<br />" +
                              $"- Mã BL: [PaysheetCode]{@event.Paysheet.Code}[/PaysheetCode], " +
                              await RenderPaysheetInfo(paysheetDto);
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.PaysheetManagement,
                TimeSheetAuditTrailAction.Create,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteUpdatePaysheetLogAsync(UpdatedPaysheetIntegrationEvent @event)
        {
            var paysheetDto = _mapper.Map<PaysheetDto>(@event.NewPaysheet);
            var log = "Cập nhật thông tin bảng lương:<br>" +
                          $"- Mã BL: {RenderPaysheetCode(@event.OldPaysheet.Code, @event.NewPaysheet.Code)}" +
                          await RenderPaysheetInfo(paysheetDto);
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.PaysheetManagement,
                TimeSheetAuditTrailAction.Update,
                log,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteCancelPaysheetLogAsync(CancelPaysheetIntegrationEvent @event)
        {
            var log = $"Hủy bảng lương: Mã: {@event.Paysheet.Code}, Tên: {@event.Paysheet.Name}, Dữ liệu tính trước ngày: {@event.Paysheet.PaysheetCreatedDate:dd/MM/yyyy HH:mm:ss}, Trạng thái: {RenderPaysheetStatusText(@event.Paysheet.PaysheetStatus)}";
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.PaysheetManagement,
                TimeSheetAuditTrailAction.Reject,
                log,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }
        public async Task WriteCreatePaysheetProcessLogAsync(CreatePaysheetProcessIntegrationEvent @event)
        {
            var paysheet = await _mediator.Send(new GetPaysheetByIdQuery(@event.PaysheetId));
            var content = "Thêm mới bảng lương:<br />" +
                          $"- Mã BL: [PaysheetCode]{@event.PaysheetCode}[/PaysheetCode], " +
                          await RenderPaysheetInfo(paysheet);
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.PaysheetManagement,
                TimeSheetAuditTrailAction.Create,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);

        }
        public async Task WriteUpdatePaysheetProcessLogAsync(UpdatePaysheetProcessIntegrationEvent @event)
        {
            var paysheet = await _mediator.Send(new GetPaysheetByIdQuery(@event.PaysheetId));
            var content = "Cập nhật dữ liệu lương thành công:<br />" +
                          $"- Mã BL: [PaysheetCode]{@event.PaysheetCode}[/PaysheetCode], " +
                          await RenderPaysheetInfo(paysheet);
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.PaysheetManagement,
                TimeSheetAuditTrailAction.Update,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }
        public async Task WriteUpdatePaysheetProcessErrorLogAsync(UpdatePaysheetProcessErrorIntegrationEvent @event)
        {
            var branch = await KiotVietInternalService.GetBranchByIdAsync(@event.PaysheetDto.BranchId, @event.PaysheetDto.TenantId);

            var log = $"Thêm mới bảng lương thất bại: <br />" + 
                            $"- Mã BL: {@event.PaysheetCode}, Tên BL: {@event.PaysheetName}, Chi nhánh: {branch?.Name}, Kỳ làm việc: {@event.PaysheetDto.StartTime:dd/MM/yyyy} - {@event.PaysheetDto.EndTime:dd/MM/yyyy}, Dữ liệu tính lương trước ngày: {@event.PaysheetDto.PaysheetCreatedDate:dd/MM/yyyy HH:mm:ss}, Trạng thái: {RenderPaysheetStatusText(@event.PaysheetStatus)} <br />" + 
                            $"Lý do thất bại: {@event.Message}";
            if (@event.IsUpdate)
                log =
                    $"Cập nhật dữ liệu lương thất bại: <br />" + 
                    $"- Mã BL: {@event.PaysheetCode}, Tên BL: {@event.PaysheetName}, Chi nhánh: {branch?.Name}, Kỳ làm việc: {@event.PaysheetDto.StartTime:dd/MM/yyyy} - {@event.PaysheetDto.EndTime:dd/MM/yyyy}, Dữ liệu tính lương trước ngày: {@event.PaysheetDto.PaysheetCreatedDate:dd/MM/yyyy HH:mm:ss}, Trạng thái: {RenderPaysheetStatusText(@event.PaysheetStatus)}  <br />" +
                    $"- Tổng lương chính: {RenderMainSalary(@event.PaysheetDto):n0}, " +
                    $"Tổng hoa hồng: {RenderCommissionSalary(@event.PaysheetDto):n0}, " +
                    $"Tổng lương làm thêm: {RenderOvertimeSalary(@event.PaysheetDto):n0}, " +
                    $"Tổng phụ cấp: {RenderAllowance(@event.PaysheetDto):n0}, " +
                    $"Tổng thưởng: {RenderBonus(@event.PaysheetDto):n0}, " +
                    $"Tổng giảm trừ: {RenderDeduction(@event.PaysheetDto):n0}, " +
                    $"Tổng lương: {RenderNetSalary(@event.PaysheetDto):n0} <br />" +
                    $"Lý do thất bại: {@event.Message}";

            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.PaysheetManagement,
                TimeSheetAuditTrailAction.Create,
                log,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }
        #region Private Methods

        private async Task<string> RenderPayslip(List<PayslipDto> payslips)
        {
            var content = string.Empty;
            if (payslips == null || !payslips.Any())
            {
                return content;
            }

            var employeeIds = payslips.Select(payslip => payslip.EmployeeId).ToList();
            var employees = await _mediator.Send(new GetEmployeeByIdsQuery(employeeIds));

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
            if (oldPaysheetString == newPaysheetString) return $"[PaysheetCode]{newPaysheetString}[/PaysheetCode], ";
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

        private static decimal RenderMainSalary(PaysheetDto paysheet)
        {
            var mainSalary = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted  && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.MainSalary);
            return mainSalary;
        }

        private static decimal RenderCommissionSalary(PaysheetDto paysheet)
        {
            var commissionSalary = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.CommissionSalary);
            return commissionSalary;
        }

        private static decimal RenderOvertimeSalary(PaysheetDto paysheet)
        {
            var overtimeSalary = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted  && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.OvertimeSalary);
            return overtimeSalary;
        }

        private static decimal RenderAllowance(PaysheetDto paysheet)
        {
            var allowance = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted  && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.Allowance);
            return allowance;
        }

        private static decimal RenderBonus(PaysheetDto paysheet)
        {
            var bonus = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.Bonus);
            return bonus;
        }

        private static decimal RenderDeduction(PaysheetDto paysheet)
        {
            var deduction = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.Deduction);
            return deduction;
        }

        private static decimal RenderNetSalary(PaysheetDto paysheet)
        {
            var netSalary = paysheet.Payslips.Where(payslip =>
                !payslip.IsDeleted && payslip.PayslipStatus != (byte)PayslipStatuses.Void &&
                payslip.PayslipStatus != (byte)PayslipStatuses.Draft).Sum(payslip => payslip.NetSalary);
            return netSalary;
        }

        private async Task<string> RenderPaysheetInfo(PaysheetDto paysheet)
        {
            var branch = await KiotVietInternalService.GetBranchByIdAsync(paysheet.BranchId, paysheet.TenantId);
            var branchName = branch?.Name ?? string.Empty;

            var payslips = paysheet.Payslips.Where(x =>
                !x.IsDeleted && x.PayslipStatus != (byte)PayslipStatuses.Void &&
                x.PayslipStatus != (byte)PayslipStatuses.Draft).ToList();

            return $"Tên BL: {paysheet.Name}, " +
                   $"Chi nhánh: {branchName}, " +
                   $"Kỳ làm việc: {paysheet.StartTime:dd/MM/yyyy} - {paysheet.EndTime:dd/MM/yyyy}, " +
                   $"Dữ liệu tính trước ngày: {paysheet.PaysheetCreatedDate:dd/MM/yyyy HH:mm:ss}," +
                   $"Trạng thái: {RenderPaysheetStatusText(paysheet.PaysheetStatus)}<br>" +
                   $"- Tổng lương chính: {RenderMainSalary(paysheet):n0}, " +
                   $"Tổng hoa hồng: {RenderCommissionSalary(paysheet):n0}, " +
                   $"Tổng lương làm thêm: {RenderOvertimeSalary(paysheet):n0}, " +
                   $"Tổng phụ cấp: {RenderAllowance(paysheet):n0}, " +
                   $"Tổng thưởng: {RenderBonus(paysheet):n0}, " +
                   $"Tổng giảm trừ: {RenderDeduction(paysheet):n0}, " +
                   $"Tổng lương: {RenderNetSalary(paysheet):n0}<br>" +
                   $"Gồm các phiếu lương: {await RenderPayslip(payslips)}";
        }
        #endregion
    }
}
