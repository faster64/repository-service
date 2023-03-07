﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents;
using KiotVietTimeSheet.Application.Queries.GetAllowanceById;
using KiotVietTimeSheet.Application.Queries.GetCommissionById;
using KiotVietTimeSheet.Application.Queries.GetDeductionById;
using KiotVietTimeSheet.Application.Queries.GetDepartmentById;
using KiotVietTimeSheet.Application.Queries.GetJobTitleById;
using KiotVietTimeSheet.Application.Queries.GetPayRateByEmployeeId;
using KiotVietTimeSheet.Application.Queries.GetShiftByIds;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Enum;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary;
using KiotVietTimeSheet.Resources;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceStack;
using Message = KiotVietTimeSheet.Resources.Message;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class EmployeeAuditProcess : BaseAuditProcess
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeAuditProcess> _logger;
        public EmployeeAuditProcess(
        IKiotVietInternalService kiotVietInternalService,
        IMediator mediator,
        IMapper mapper,
        ILogger<EmployeeAuditProcess> logger
        ) : base(kiotVietInternalService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task WriteCreateEmployeeLogAsync(CreatedEmployeeIntegrationEvent @event)
        {
            _logger.LogInformation("thêm nhân viên");
            var employeeInfo = @event.Employee;
            var employeePayRate = await _mediator.Send(new GetPayRateByEmployeeIdQuery(@event.Employee.Id));
            var branches = await GetBranchWithEmployeeAndPayRate(employeeInfo, employeePayRate, @event.WorkBranchIds);
            var employeeInfoAuditLog = await RenderCreateEmployeeAuditlog(employeeInfo, @event, branches);
            var employeePayRateAuditLog =
                employeePayRate != null ? await RenderCreatePayRateAuditLog(employeePayRate, branches) : "";
            var content = $"{employeeInfoAuditLog}{employeePayRateAuditLog}";
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.EmployeeManagement,
                TimeSheetAuditTrailAction.Create,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteUpdateEmployeeLogAsync(UpdatedEmployeeIntegrationEvent @event)
        {
            var oldEmployee = @event.OldEmployee;
            var newEmployee = @event.NewEmployee;
            var workBranchIds = @event.WorkBranchIds;
            var employeePayRate = _mapper.Map<PayRateDto>(@event.PayRate);
            var branches = await GetBranchWithEmployeeAndPayRate(newEmployee, employeePayRate, @event.WorkBranchIds);
            var employeeSalaryBranch = RenderEmployeeSalaryBranchName(newEmployee, branches);
            var employeeBranchWorking = RenderEmployeeBranchWorkingName(workBranchIds, branches);
            var employeeName = RenderEmployeeName(newEmployee);

            var employeeInforAuditLog = await RenderEmployeeInforAuditLog(newEmployee);
            var codeAudit = oldEmployee.Code != newEmployee.Code
                ? $"Mã {oldEmployee.Code} -> [EmployeeCode]{newEmployee.Code}[/EmployeeCode]"
                : $"Mã [EmployeeCode]{oldEmployee.Code}[/EmployeeCode]";
            var deviceChange =  oldEmployee.IdentityKeyClocking != newEmployee.IdentityKeyClocking && string.IsNullOrWhiteSpace(newEmployee.IdentityKeyClocking)
                          ? $"<br>- {Message.delDeviceResult}: {oldEmployee.IdentityKeyClocking} <br>- {Message.regDeviceResult}: <strong>{Label.no}</strong><br>"
                          : string.Empty;

            var employeeInforAuditLogItems =
                new List<string>
                    {
                                "Cập nhật thông tin nhân viên:" +
                                $"<br/>- {codeAudit}",
                                $"Tên nhân viên: {employeeName}",
                                $"Chi nhánh trả lương: {employeeSalaryBranch}" ,
                                $"Chi nhánh làm việc: {employeeBranchWorking}" ,
                                employeeInforAuditLog
                    }
                    .Where(x => !string.IsNullOrEmpty(x));

            var employeePayRateAuditLog =
                employeePayRate != null ? await RenderCreatePayRateAuditLog(employeePayRate, branches) : "";
            var auditLogContent = string.Join(", ", employeeInforAuditLogItems) + employeePayRateAuditLog + deviceChange;
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.EmployeeManagement,
                TimeSheetAuditTrailAction.Update,
                auditLogContent,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteDeleteEmployeeLogAsync(DeletedEmployeeIntegrationEvent @event)
        {
            var log = $"Xóa nhân viên: {@event.Employee.Code}, Tên NV: {@event.Employee.Name}";
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.EmployeeManagement,
                TimeSheetAuditTrailAction.Delete,
                log,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteDeleteMultipleEmployeeLogAsync(DeletedMultipleEmployeeIntegrationEvent @event)
        {
            var employees = @event.ListEmployees;
            var employeesAuditContent = employees
                .Select(RenderDeleteEmployeeAuditAsync);
            var log = $"Xóa nhân viên: {string.Join("", employeesAuditContent)}";
            await AddLogAsync(
                GenerateLog(
                    TimeSheetFunctionTypes.EmployeeManagement,
                    TimeSheetAuditTrailAction.Delete,
                    log,
                    @event.Context
                )
            );
        }
        
        public async Task WriteUpdateEmployeeDeviceLogAsync(UpdateEmployeeDeviceIntegrationEvent @event)
        {
            var content = $"{Message.updateEmployee} <br>" + 
                          $"- {Label.employee_code}: {@event.EmployeeCode} <br>" +
                          $"- {Message.deviceName}: {@event.IdentityKeyClocking} <br>" +
                          $"- {Message.regDeviceResult}: <strong>{Label.yes}</strong>";
            if (@event.Context.UserId == 0) @event.Context.SetUserId(@event.UserId);
            if (@event.Context.BranchId == 0) @event.Context.SetBranchId(@event.BranchId);
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.EmployeeManagement,
                TimeSheetAuditTrailAction.Update,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        #region Private Methods
        private static string RenderEmployeeGender(bool? value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value == true ? "Nam" : "Nữ";
        }
        private async Task<string> RenderCreateEmployeeAuditlog(Employee employee, CreatedEmployeeIntegrationEvent @event, List<BranchDto> branches)
        {
            var employeeCode = RenderEmployeeCode(employee);
            var employeeName = RenderEmployeeName(employee);
            var employeeSalaryBranch = RenderEmployeeSalaryBranchName(employee, branches);
            var employeeBranchWorking = RenderEmployeeBranchWorkingName(@event.WorkBranchIds, branches);
            var employeeInforAuditLog = await RenderEmployeeInforAuditLog(employee, false);

            return "Thêm mới nhân viên: " +
                   $"<br/>- Mã nhân viên: {employeeCode}" +
                   $", Tên nhân viên: {employeeName}" +
                   $", Chi nhánh trả lương: {employeeSalaryBranch}" +
                   $", Chi nhánh làm việc: {employeeBranchWorking}" +
                   employeeInforAuditLog;
        }

        private async Task<string> RenderEmployeeInforAuditLog(Employee employee, bool isUpdateEmployee = true)
        {
            var employeeDateOfBirth = RenderEmployeeDoB(employee);
            var employeeGender = RenderEmployeeGender(employee.Gender);
            var employeeIdentityNumber = RenderEmployeeIdentity(employee);
            var employeeDepartment = await RenderEmployeeDepartment(employee);
            var employeeJobTitle = await RenderEmployeeJobTitle(employee);
            var employeeUser = await RenderEmployeeUser(employee);
            var employeeMobilePhone = RenderEmployeeMobilePhone(employee);
            var employeeEmail = RenderEmployeeEmail(employee);
            var employeeFacebook = RenderEmployeeFacebook(employee);
            var employeeAdress = RenderEmployeeAddress(employee);
            var employeeWardName = RenderEmployeeWardName(employee);
            var employeeLocationName = RenderEmployeeLocation(employee);
            var employeeNote = RenderEmployeeNote(employee);

            var dateOfBirth = string.Empty;
            var gender = string.Empty;
            var identityNumber = string.Empty;
            var department = string.Empty;
            var jobTitle = string.Empty;
            var user = string.Empty;
            var mobilePhone = string.Empty;
            var email = string.Empty;
            var faceBook = string.Empty;
            var address = string.Empty;
            var locationName = string.Empty;
            var wardName = string.Empty;
            var note = string.Empty;

            var ortherString = "";
            if (isUpdateEmployee) ortherString = ", ";

            if (!string.IsNullOrEmpty(employeeDateOfBirth))
                dateOfBirth = ortherString + $"Ngày sinh: {employeeDateOfBirth}";
            if (!string.IsNullOrEmpty(employeeGender)) gender =
                ortherString + $"Giới tính: {employeeGender}";
            if (!string.IsNullOrEmpty(employeeIdentityNumber))
                identityNumber = ortherString + $"Số CMTND: {employeeIdentityNumber}";
            if (!string.IsNullOrEmpty(employeeDepartment))
                department = ortherString + $"Phòng ban: {employeeDepartment}";
            if (!string.IsNullOrEmpty(employeeJobTitle))
                jobTitle = ortherString + $"Chức danh: {employeeJobTitle}";
            if (!string.IsNullOrEmpty(employeeUser))
                user = ortherString + $"Người dùng: {employeeUser}";
            if (!string.IsNullOrEmpty(employeeMobilePhone))
                mobilePhone = ortherString + $"Số điện thoại: {employeeMobilePhone}";
            if (!string.IsNullOrEmpty(employeeEmail))
                email = ortherString + $"Email: {employeeEmail}";
            if (!string.IsNullOrEmpty(employeeFacebook))
                faceBook = ortherString + $"Facebook: {employeeFacebook}";
            if (!string.IsNullOrEmpty(employeeAdress))
                address = ortherString + $"Địa chỉ: {employeeAdress}";
            if (!string.IsNullOrEmpty(employeeLocationName))
                locationName = ortherString + $"Khu vực: {employeeLocationName}";
            if (!string.IsNullOrEmpty(employeeWardName))
                wardName = ortherString + $"Phường xã: {employeeWardName}";
            if (!string.IsNullOrEmpty(employeeNote))
                note = ortherString + $"Ghi chú: {employeeNote}";

            var employeeInforAuditLogItems = new List<string>
                {
                    $"{dateOfBirth}",
                    $"{gender}",
                    $"{identityNumber}",
                    $"{department}",
                    $"{jobTitle}",
                    $"{user}",
                    $"{mobilePhone}",
                    $"{email}",
                    $"{faceBook}",
                    $"{address}",
                    $"{locationName}",
                    $"{wardName}",
                    $"{note}"
                }
                .Where(x => !string.IsNullOrEmpty(x));

            var employeeInforAuditLog = string.Join(", ", employeeInforAuditLogItems);
            return employeeInforAuditLog;
        }

        private async Task<string> RenderCreatePayRateAuditLog(PayRateDto payRate, List<BranchDto> branches)
        {
            var salaryPeriod = RenderSalaryPeriodAuditLog(payRate);
            var mainSalary = await RenderMainSalaryAuditLog(payRate, branches);
            var commissionSalary = RenderCommissionSalaryAuditLog(payRate, branches);
            var overtimeSalary = RenderOvertimeSalaryAuditLog(payRate);
            var allowance = RenderAllowanceSalaryAuditLog(payRate);
            var deduction = RenderDeductionSalaryAuditLog(payRate);

            return $"<br/>{salaryPeriod}" +
                   $"<br/>{mainSalary}" +
                   $"<br/>{commissionSalary}" +
                   $"<br/>{overtimeSalary}" +
                   $"<br/>{allowance}" +
                   $"<br/>{deduction}";
        }

        private async Task<string> RenderMainSalaryAuditLog(PayRateDto payRate, List<BranchDto> branches)
        {
            var payRateDetailItem = payRate?.PayRateDetails?.FirstOrDefault(x => x.RuleType == nameof(MainSalaryRule));
            if (payRateDetailItem == null)
                return @"- Lương chính: Không áp dụng";

            var template = new StringBuilder();
            var ruleValue = JsonConvert.DeserializeObject<MainSalaryRuleValue>(payRateDetailItem.RuleValue);
            var ruleType = ruleValue.Type;
            var ruleTypeName = RenderMainRuleTypeName(ruleType);
            template.Append(ruleTypeName);
            var shiftIds = ruleValue.MainSalaryValueDetails.Select(shift => shift.ShiftId).ToList();

            var shifts = await _mediator.Send(new GetShiftByIdsQuery(shiftIds));

            var isShowAdvance = GetIsShowAdvance(ruleValue.MainSalaryValueDetails);

            var numberDayWorkingName = string.Empty;
            if (ruleType == MainSalaryTypes.Day) numberDayWorkingName = $"/Số ngày công chuẩn";

            branches ??= new List<BranchDto>();

            foreach (var mainSalaryValueDetailsItem in ruleValue.MainSalaryValueDetails)
            {
                var branchName = string.Empty;
                var shiftName = "Mặc định";

                var shift = shifts.FirstOrDefault(s => s.Id == mainSalaryValueDetailsItem.ShiftId);
                var branch = branches.FirstOrDefault(b => b.Id == shift?.BranchId);
                if (branch != null) branchName = $" ({branch.Name})";
                if (shift != null) shiftName = $"{shift.Name}{branchName}";

                var tplHolidays = RenderTplHoliDay(mainSalaryValueDetailsItem.MainSalaryHolidays);

                if (isShowAdvance)
                {
                    template.Append($"<br/>&nbsp;&nbsp;" + shiftName +
                                $": {mainSalaryValueDetailsItem.Default:n0}{numberDayWorkingName}" + tplHolidays);
                }
                else
                {
                    template.Append($" {mainSalaryValueDetailsItem.Default:n0}");
                }
            }
            return template.ToString();
        }

        private bool GetIsShowAdvance(List<MainSalaryRuleValueDetail> mainSalaryValueDetails)
        {
            if (mainSalaryValueDetails == null || mainSalaryValueDetails.Count == 0) return false;
            if (mainSalaryValueDetails.Count > 1) return true;
            var mainSalaryHolidays = mainSalaryValueDetails[0].MainSalaryHolidays;
            var salaryDefault = mainSalaryValueDetails[0].Default;
            foreach (var dayHoliday in mainSalaryHolidays)
            {
                if (!dayHoliday.IsApply) continue;

                if (dayHoliday.MoneyTypes == MoneyTypes.Money &&
                    dayHoliday.Value != salaryDefault)
                {
                    return true;
                }
                else if (dayHoliday.MoneyTypes == MoneyTypes.Percent)
                {
                    var valueDays = (salaryDefault * dayHoliday.Value) / 100;
                    if (salaryDefault != valueDays) return true;
                }
            }

            return false;
        }
        private string RenderEmployeeCode(Employee employee) => $"[EmployeeCode]{employee.Code}[/EmployeeCode]";
        private string RenderEmployeeName(Employee employee) => employee?.Name ?? string.Empty;
        private string RenderEmployeeDoB(Employee employee) => employee.DOB?.ToString("dd/MM/yyyy") ?? string.Empty;
        private string RenderEmployeeIdentity(Employee employee) => employee?.IdentityNumber ?? string.Empty;
        private string RenderEmployeeMobilePhone(Employee employee) => employee?.MobilePhone ?? string.Empty;
        private string RenderEmployeeEmail(Employee employee) => employee?.Email ?? string.Empty;
        private string RenderEmployeeFacebook(Employee employee) => employee?.Facebook ?? string.Empty;
        private string RenderEmployeeAddress(Employee employee) => employee?.Address ?? string.Empty;
        private string RenderEmployeeWardName(Employee employee) => employee?.WardName ?? string.Empty;
        private string RenderEmployeeLocation(Employee employee) => employee?.LocationName ?? string.Empty;
        private string RenderEmployeeNote(Employee employee) => employee?.Note ?? string.Empty;
        private async Task<string> RenderEmployeeDepartment(Employee employee)
        {
            var departmentId = employee.DepartmentId ?? 0;
            return (await _mediator.Send(new GetDepartmentByIdQuery(departmentId)))?.Name ?? string.Empty;
        }

        private async Task<string> RenderEmployeeJobTitle(Employee employee)
        {
            var jobTitleId = employee.DepartmentId ?? 0;
            return (await _mediator.Send(new GetJobTitleByIdQuery(jobTitleId)))?.Name ?? string.Empty;
        }

        private async Task<string> RenderEmployeeUser(Employee employee) => employee.UserId != null
            ? (await KiotVietInternalService.GetUserByIdsAsync(new List<long> { employee.UserId.GetValueOrDefault() }, employee.TenantId)).First()?.Name ?? string.Empty
            : string.Empty;
        private string RenderSalaryPeriodAuditLog(PayRateDto payRate) => $"- Kì hạn trả lương: {((SalaryPeriod)payRate.SalaryPeriod).ToDescription()}";

        private string RenderCommissionSalaryAuditLog(PayRateDto payRate, List<BranchDto> branches)
        {
            var commissionSpaceChild = "&nbsp;&nbsp";
            var commissionSalaryRule = payRate?.PayRateDetails?.FirstOrDefault(x => x.RuleType == nameof(CommissionSalaryRuleV2));
            if (commissionSalaryRule == null) return "- Hoa hồng: Không áp dụng";

            var ruleObj = RuleFactory.GetRule(commissionSalaryRule.RuleType, commissionSalaryRule.RuleValue);
            var ruleValue = ruleObj.GetRuleValue() as CommissionSalaryRuleValueV2;
            if (ruleValue?.CommissionSalaryRuleValueDetails == null ||
                ruleValue.CommissionSalaryRuleValueDetails.Count == 0)
                return "- Hoa hồng: Không áp dụng";

            var useMinCommission = ruleValue.UseMinCommission;
            var minCommission = ruleValue.MinCommission;
            var formTypeName = RenderCommissionFormalityTypeName(ruleValue.FormalityTypes);
            var ruleTypeName = RenderCommissionTypeName(ruleValue.Type);
            var commissionBranchName = RenderCommissionSalaryBranchName(ruleValue, branches);
            var minCommissionStr = useMinCommission ? $"<br/>Tối thiểu: {minCommission:n0}" : string.Empty;
            var listCommissionLevelDetailAudit = ruleValue.CommissionSalaryRuleValueDetails.OrderBy(x => x.CommissionLevel)
                .Select(async x =>
                {
                    var name = "doanh thu vượt";
                    if (ruleValue.Type == CommissionSalaryTypes.WithTotalPersonalGrossProfit)
                    {
                        name = "lợi nhuận gộp";
                    }
                    var commissionFactor = x.ValueRatio == null
                        ? $"{x.Value.GetValueOrDefault():n0} VND"
                        : $"{x.ValueRatio.GetValueOrDefault():n2}% {name}";
                    var commissionTableName = x.CommissionTableId != null && x.CommissionTableId > 0
                        ? await GetCommissionTableName(x.CommissionTableId.GetValueOrDefault())
                        : null;
                    return $"</br>{commissionSpaceChild}Từ {x.CommissionLevel:n0} - {commissionTableName ?? commissionFactor}";
                })
                .Select(x => x.Result);

            return $"- Hoa hồng: {formTypeName} </br>" +
                   $"{(string.IsNullOrEmpty(commissionBranchName) ? string.Empty : $"{commissionSpaceChild}<span style='display: inline-block;width: 95%;'>Chi nhánh: {commissionBranchName} </span></br>")}" +
                   $"{commissionSpaceChild}Hình thức: {ruleTypeName} {minCommissionStr}{string.Join("", listCommissionLevelDetailAudit)}";
        }

        private string RenderCommissionSalaryBranchName(CommissionSalaryRuleValueV2 ruleValueV2, List<BranchDto> branches)
        {
            branches ??= new List<BranchDto>();

            if (ruleValueV2.FormalityTypes != CommissionSalaryFormalityTypes.BranchCommissionRevenue)
                return string.Empty;
            if (ruleValueV2.IsAllBranch) return $"Tất cả chi nhánh </br>";
            if (ruleValueV2.BranchIds == null || ruleValueV2.BranchIds.Count == 0) return string.Empty;
            return string.Concat(string.Join(", ",
                branches.Where(b => ruleValueV2.BranchIds.Contains((int)b.Id)).Select(x => x.Name).ToList()), "</br>");
        }
        private string RenderMainRuleTypeName(MainSalaryTypes type)
        {
            switch (type)
            {
                case MainSalaryTypes.Hour: return "- Lương chính tính theo giờ làm việc thực tế:";
                case MainSalaryTypes.Shift: return "- Lương chính tính theo ca làm việc thực tế:";
                case MainSalaryTypes.Day: return "- Lương chính tính theo ngày công chuẩn:";
                case MainSalaryTypes.Fixed: return "- Lương chính cố định:";
                default: return "- Lương chính: Không áp dụng";
            }
        }
        private string RenderCommissionTypeName(CommissionSalaryTypes type)
        {
            switch (type)
            {
                case CommissionSalaryTypes.WithTotalCommission:
                    return "Tính theo mức tổng doanh thu";
                case CommissionSalaryTypes.WithMinimumCommission:
                    return "Tính theo mức vượt doanh thu tối thiểu";
                case CommissionSalaryTypes.WithLevelCommission:
                    return "Tính theo nấc bậc thang tổng doanh thu";
                case CommissionSalaryTypes.WithTotalPersonalGrossProfit:
                    return "Tính theo mức tổng lợi nhuận gộp";
                default:
                    return string.Empty;
            }
        }
        private string RenderCommissionFormalityTypeName(CommissionSalaryFormalityTypes type)
        {
            switch (type)
            {
                case CommissionSalaryFormalityTypes.PersonalCommissionRevenue:
                    return "Theo doanh thu cá nhân";
                case CommissionSalaryFormalityTypes.BranchCommissionRevenue:
                    return "Theo doanh thu chi nhánh";
                case CommissionSalaryFormalityTypes.PersonalGrossProfit:
                    return "Theo lợi nhuận gộp cá nhân";
                default:
                    return string.Empty;
            }
        }
        private string RenderOvertimeSalaryAuditLog(PayRateDto payRate)
        {
            var overtimeSalaryRule = payRate?.PayRateDetails?.FirstOrDefault(x => x.RuleType == nameof(OvertimeSalaryRule));
            if (overtimeSalaryRule == null) return "- Lương làm thêm giờ: Không áp dụng";
            var ruleObj = RuleFactory.GetRule(overtimeSalaryRule.RuleType, overtimeSalaryRule.RuleValue);
            OvertimeSalaryRuleValue ruleValue = ruleObj.GetRuleValue() as OvertimeSalaryRuleValue;
            var template = new StringBuilder($"- Lương làm thêm giờ: <br />");
            if (ruleValue != null)
            {
                var count = 0;
                foreach (OvertimeSalaryDays overTimeSalaryItem in ruleValue.OvertimeSalaryDays)
                {
                    if (overTimeSalaryItem.IsApply)
                    {
                        var moneyType = overTimeSalaryItem.MoneyTypes == MoneyTypes.Percent ?
                            MoneyTypes.Percent.GetType().GetMember(MoneyTypes.Percent.ToString()).First().GetCustomAttribute<DisplayAttribute>()?.GetName() : "";
                        if (count > 0) template.Append(" - ");
                        count++;
                        template.Append($"{overTimeSalaryItem.Type.ToDescription()}: " + $"{overTimeSalaryItem.Value:n0}" + moneyType);
                    }
                }
            }
            return template.ToString();
        }
        private string RenderAllowanceSalaryAuditLog(PayRateDto payRate)
        {
            var allowanceSalaryRule = payRate?.PayRateDetails?.FirstOrDefault(x => x.RuleType == nameof(AllowanceRule));
            if (allowanceSalaryRule == null) return "- Phụ cấp: Không áp dụng";

            var ruleObj = RuleFactory.GetRule(allowanceSalaryRule.RuleType, allowanceSalaryRule.RuleValue);
            var ruleValue = ruleObj.GetRuleValue() as AllowanceRuleValue;
            if (ruleValue?.AllowanceRuleValueDetails == null || ruleValue.AllowanceRuleValueDetails.Count == 0) return "- Phụ cấp: Không áp dụng";

            var listAllowanceRuleAudit = ruleValue.AllowanceRuleValueDetails
                .Select(async x =>
                {
                    var allowanceValueStr = x.ValueRatio == null
                        ? $"{x.Value.GetValueOrDefault():n0}"
                        : $"{x.ValueRatio.GetValueOrDefault():n2}% lương chính";
                    return $"</br>{await RenderAllowanceName(x.AllowanceId)}: {allowanceValueStr} ({x.Type.ToDescription()})";
                })
                .Select(x => x.Result);
            return $"- Phụ cấp: {string.Join("", listAllowanceRuleAudit)}";
        }
        private async Task<string> RenderAllowanceName(long allowanceId)
        {
            return (await _mediator.Send(new GetAllowanceByIdQuery(allowanceId)))?.Name ?? "Phụ cấp không xác định";
        }

        private string RenderDeductionSalaryAuditLog(PayRateDto payRate)
        {
            var deductionSalaryRule = payRate?.PayRateDetails?.FirstOrDefault(x => x.RuleType == nameof(DeductionRule));
            if (deductionSalaryRule == null) return "- Giảm trừ: Không áp dụng";

            var ruleObj = RuleFactory.GetRule(deductionSalaryRule.RuleType, deductionSalaryRule.RuleValue);
            var ruleValue = ruleObj.GetRuleValue() as DeductionRuleValue;
            if (ruleValue?.DeductionRuleValueDetails == null || ruleValue.DeductionRuleValueDetails.Count == 0) return "- Giảm trừ: Không áp dụng";

            var listDeductionRuleAudit = ruleValue.DeductionRuleValueDetails
                .Select(async x =>
                {
                    var deductionValueStr = string.Empty;
                    if (x.ValueRatio != null)
                    {
                        deductionValueStr = $"{x.ValueRatio.GetValueOrDefault():n2}% tổng thu nhập";
                    }
                    else
                    {
                        if (x.Type != DeductionTypes.Early && x.Type != DeductionTypes.Late)
                        {
                            deductionValueStr = $"{x.Value.GetValueOrDefault():n0}";
                        }
                        else
                        {

                            string rule = GetRuleMessage(x.DeductionRuleId);
                            deductionValueStr = x.DeductionTypeId == (int)DeductionRuleTypes.Time ? $" {x.Value.GetValueOrDefault():n0}/{x.BlockTypeTimeValue} lần - {rule}" : $" Block = {x.Value.GetValueOrDefault():n0}/{x.BlockTypeMinuteValue} phút - {rule}";
                        }
                    }
                    return $"</br>{await RenderDeductionName(x.DeductionId)}: {deductionValueStr}";
                })
                .Select(x => x.Result);
            return $"- Giảm trừ: {string.Join("", listDeductionRuleAudit)}";
        }

        private string GetRuleMessage(int? deductionTypeId)
        {
            var rule = deductionTypeId == (int)DeductionTypes.Late ? "Tính theo đi muộn" : "Tính theo về sớm";
            return rule;
        }
        private async Task<string> RenderDeductionName(long deductionId)
        {
            return (await _mediator.Send(new GetDeductionByIdQuery(deductionId)))?.Name ?? "Giảm trừ không xác định";
        }

        private string RenderDeleteEmployeeAuditAsync(Employee employee)
        {
            var employeeCode = employee.Code;
            var employeeName = employee.Name;
            var content = $"</br>Mã NV: {employeeCode}, Tên NV: {employeeName}";
            return content;
        }

        private async Task<string> GetCommissionTableName(long commissionTableId)
        {
            return (await _mediator.Send(new GetCommissionByIdQuery(commissionTableId)))?.Name ?? "Bảng hoa hồng không xác định";
        }

        private async Task<List<BranchDto>> GetBranchWithEmployeeAndPayRate(Employee employee, PayRateDto payRate, List<int> workBranchIds)
        {
            var branchIds = new List<int>();

            if (workBranchIds != null) branchIds.AddRange(workBranchIds);
            branchIds.Add(employee.BranchId);

            var commissionSalaryRule = payRate?.PayRateDetails?.FirstOrDefault(x => x.RuleType == nameof(CommissionSalaryRuleV2));
            if (commissionSalaryRule != null)
            {
                var ruleObj = RuleFactory.GetRule(commissionSalaryRule.RuleType, commissionSalaryRule.RuleValue);
                if (ruleObj?.GetRuleValue() is CommissionSalaryRuleValueV2 ruleValue && !ruleValue.IsAllBranch)
                    branchIds.AddRange(ruleValue.BranchIds);
            }
            branchIds = branchIds.Distinct().ToList();

            var branches = branchIds.Count > 0
                ? await KiotVietInternalService.GetBranchByIdsAsync(branchIds, employee.TenantId)
                : new List<BranchDto>();
            return branches;
        }

        private string RenderEmployeeSalaryBranchName(Employee employee, List<BranchDto> branches)
        {
            branches ??= new List<BranchDto>();
            return branches.FirstOrDefault(x => x.Id == employee.BranchId)?.Name ?? string.Empty;
        }
            
            
        private string RenderEmployeeBranchWorkingName(List<int> workBranchIds, List<BranchDto> branches)
        {
            if (workBranchIds == null) return string.Empty;
            branches ??= new List<BranchDto>();


            var getBranchWorkings = workBranchIds.Join(branches,
                branchWorkingIds => branchWorkingIds,
                branch => branch.Id,
                (branchWorkingIds, branch) => new BranchDto { Id = branchWorkingIds, Name = branch.Name });
            return string.Join(", ", getBranchWorkings.Select(x => x.Name));
        }
        private string RenderTplHoliDay(List<MainSalaryHolidays> mainSalaryHolidays)
        {
            var tplHolidays = new StringBuilder();
            foreach (var mainSalaryHolidaysItem in mainSalaryHolidays)
            {
                var moneyType = string.Empty;
                if (mainSalaryHolidaysItem.MoneyTypes == MoneyTypes.Percent)
                {
                    moneyType = MoneyTypes.Percent.GetType().GetMember(MoneyTypes.Percent.ToString()).First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName();
                }

                if (mainSalaryHolidaysItem.IsApply)
                {
                    tplHolidays.Append($" - " + mainSalaryHolidaysItem.Type.ToDescription()
                                              + $": {mainSalaryHolidaysItem.Value:n0}" + moneyType);
                }
            }
            return tplHolidays.ToString();
        }
        #endregion
    }
}
