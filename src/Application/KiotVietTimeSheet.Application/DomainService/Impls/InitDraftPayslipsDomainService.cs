using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.SharedKernel.Specifications;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class InitDraftPayslipsDomainService : IInitDraftPayslipsDomainService
    {
        #region Properties
        private readonly IHolidayReadOnlyRepository _holidayReadOnlyRepository;
        private readonly IBranchSettingReadOnlyRepository _branchSettingReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IEmployeeBranchReadOnlyRepository _employeeBranchReadOnlyRepository;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IPayRateReadOnlyRepository _payRateReadOnlyRepository;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IAuthService _authService;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;
        private readonly IMapper _mapper;
        #endregion

        public InitDraftPayslipsDomainService(
            IHolidayReadOnlyRepository holidayReadOnlyRepository,
            IBranchSettingReadOnlyRepository branchSettingReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IEmployeeBranchReadOnlyRepository employeeBranchReadOnlyRepository,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IPayRateReadOnlyRepository payRateReadOnlyRepository,
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IAuthService authService,
            IKiotVietServiceClient kiotVietServiceClient,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService,
            IMapper mapper
            )
        {
            _holidayReadOnlyRepository = holidayReadOnlyRepository;
            _branchSettingReadOnlyRepository = branchSettingReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _employeeBranchReadOnlyRepository = employeeBranchReadOnlyRepository;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _payRateReadOnlyRepository = payRateReadOnlyRepository;
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _authService = authService;
            _kiotVietServiceClient = kiotVietServiceClient;
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
            _mapper = mapper;

        }

        #region Public methods
        //public async Task<bool> 

        /// <inheritdoc />
        public async Task<List<DraftPayslipDomainServiceDto>> InitDraftPayslipsAsync(List<Payslip> payslips, List<long> employeeIds, DateTime from,
            DateTime to, byte salaryPeriod, int standardWorkingDayNumber, int timeOfStandardWorkingDay, int branchId, long? payslipCreatedBy = null,
            DateTime? payslipCreatedDate = null, SettingsToObject settings = null)
        {
            var tenantId = _authService.Context.TenantId;
            var isUpdate = payslips != null;
            // Lấy danh sách nhân viên
            var employees = await GetEmployees(payslips, employeeIds, branchId, isUpdate);

            employees = employees.OrderBy(x => x.Name.Split(' ').Last()).ToList();
            var listEmployeeId = employees.Select(e => e.Id).ToList();

            //Lấy danh sách chi nhánh làm việc của nhân viên
            var lstBranchWorking =
                await _employeeBranchReadOnlyRepository.GetBySpecificationAsync(
                    new FindBranchByEmployeeIdsSpec(listEmployeeId));
            employees.ForEach(e =>
            {
                var lstBranchWithEmployee = lstBranchWorking.Where(x => x.EmployeeId == e.Id).ToList();
                e.EmployeeBranches = lstBranchWithEmployee;
            });



            // Lấy danh sách clocking đã chấm công ra và nghỉ có phép
            var clockings = await _clockingReadOnlyRepository.GetClockingForPaySheet(from, to, listEmployeeId);

            // Lấy danh sách clocking chưa chốt lương
            var unPaidClockings = clockings.Where(c => c.ClockingPaymentStatus == (byte)ClockingPaymentStatuses.UnPaid).ToList();

            // Lấy danh sách clocking đã chốt lương
            var paidClockings = clockings.Where(c => c.ClockingPaymentStatus == (byte)ClockingPaymentStatuses.Paid).ToList();

            //lấy danh sách vi phạm
            var clockingsPenalizesDto =  await _clockingReadOnlyRepository.GetClockingPenalizeForPaySheet(from, to, listEmployeeId);
            var clockingsPenalize = _mapper.Map<List<ClockingPenalize>>(clockingsPenalizesDto);

            // Lấy danh sách holiday
            var holidays = await _holidayReadOnlyRepository.GetBySpecificationAsync(
                        (new FindHolidayByFromGreaterThanOrEqualSpec(from).And(new FindHolidayByFromLessThanOrEqualSpec(to)))
                            .Or(new FindHolidayByToGreaterThanOrEqualSpec(from).And(new FindHolidayByToLessThanOrEqualSpec(to))));

            // Lấy danh sách ca
            var shifts = await _shiftReadOnlyRepository.GetBySpecificationAsync(new FindShiftByBranchIdSpec(branchId));
            shifts = shifts.Where(x => !x.IsDeleted).ToList();

            // Lấy settings của chi nhánh
            var branchSetting = await _branchSettingReadOnlyRepository.FindBranchSettingWithDefault(new FindBranchSettingByBranchIdSpec(branchId));

            // Lấy danh sách payrate
            var payRates = await _payRateReadOnlyRepository.GetBySpecificationAsync(new FindPayRateByEmployeeIdsSpec(listEmployeeId), true);

            // Lấy doanh thu của nhân viên
            var lstUser = employees.Where(e => (e.UserId ?? 0) > 0).Select(e => e.UserId ?? 0).ToList();

            var userRevenues = new List<UserByRevenueObject>();
            var userProductRevenues = new List<ProductRevenue>();
            var userGrossProfits = new List<UserByRevenueObject>();

            if (lstUser.Any())
            {
                userRevenues = await _kiotVietServiceClient.GetUserByRevenue(
                    tenantId,
                    null,
                    lstUser,
                    from.Date,
                    to.Date.AddDays(1).AddTicks(-1));

                // Lấy doanh thu theo sản phẩm của nhân viên
                userProductRevenues = await _kiotVietServiceClient.GetProductRevenueByUser(
                    tenantId,
                    null,
                    lstUser,
                    from.Date,
                    to.Date.AddDays(1).AddTicks(-1));

                userGrossProfits = await _kiotVietServiceClient.GetUserByGrossProfit(
                    tenantId,
                    null,
                    lstUser,
                    from.Date,
                    to.Date.AddDays(1).AddTicks(-1));
            }

            //start Kiểm tra nếu có bất kỳ nhân viên nào có cài đặt tính hoa hồng cho tất cả chi nhánh
            var payRateDetails = payRates.Select(p => p.PayRateDetails).ToList();

            var branchIds = new List<long>();
            var commissionIds = new List<long>();
            var commissionSalaryRuleValue = payRateDetails.Select(GetCommissionRuleValue).ToList();
            commissionSalaryRuleValue = commissionSalaryRuleValue.Where(c => c.CommissionSalaryRuleValueDetails != null).ToList();
            foreach (var commissionSalaryValue in commissionSalaryRuleValue)
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
            //end

            //Lấy doanh thu theo chi nhánh
            var branchProductRevenues = new List<ProductRevenue>();
            if (commissionSalaryRuleValue.Any(c => c.FormalityTypes == CommissionSalaryFormalityTypes.BranchCommissionRevenue))
            {
                var isAllBranch = commissionSalaryRuleValue.Any(c => c.IsAllBranch && c.FormalityTypes == CommissionSalaryFormalityTypes.BranchCommissionRevenue);
                branchProductRevenues = await GetBranchProductRevenues(isAllBranch, branchIds, from, to);
            }

            //Lấy danh sách bảng hoa hồng được gấn cho nhân viên
            commissionIds = commissionIds.Where(c => c != 0).Distinct().ToList();

            var commissionTables = await _commissionReadOnlyRepository.GetBySpecificationAsync(
                new FindCommissionByCommissionIdsSpec(commissionIds), true, true);

            // Lấy danh sách giảm trừ 
            var deductions = await _deductionReadOnlyRepository.GetBySpecificationAsync(new FindDeductionByTenantIdSpec(tenantId));

            // Lấy danh sách phụ cấp 
            var allowances = await _allowanceReadOnlyRepository.GetBySpecificationAsync(new FindAllowanceByTenantIdSpec(tenantId));
            // Tạo Payslips
            var draftPayslips = new List<DraftPayslipDomainServiceDto>();
            employees.ForEach(employee =>
            {
                var mapperCommissionTables = commissionTables.Select(x => new CommissionTableParam(x)).ToList();
                employee.Clockings = unPaidClockings.Where(x => x.EmployeeId == employee.Id).ToList();
                var payRate = payRates.FirstOrDefault(p => p.EmployeeId == employee.Id && p.SalaryPeriod == salaryPeriod);
                if (!_paySheetOutOfDateDomainService.IsCheckPayRateDetail(payRate, allowances, deductions) ) return;
                var payslipClockingPenalizes = clockingsPenalize.Where(x => x.EmployeeId == employee.Id).ToList();

                var rules = payRate?.PayRateDetails.Select(prd => RuleFactory.GetRule(prd.RuleType, prd.RuleValue)).ToList();
                rules = RuleFactory.OrderRules(rules);

                var engineResource = new EngineResource
                {
                    Employee = employee,
                    BranchSetting = branchSetting,
                    UnPaidClockings = unPaidClockings.Where(c => c.EmployeeId == employee.Id).ToList(),
                    PaidClockings = paidClockings.Where(c => c.EmployeeId == employee.Id).ToList(),
                    ClockingPenalizes = payslipClockingPenalizes,
                    Holidays = holidays,
                    TimeOfStandardWorkingDay = timeOfStandardWorkingDay,
                    StandardWorkingDayNumber = standardWorkingDayNumber,
                    TotalRevenue = userRevenues.FirstOrDefault(u => u.EmployeeId == employee.Id)?.NetSale ?? 0,
                    TotalGrossProfit = userGrossProfits.FirstOrDefault(u => u.EmployeeId == employee.Id)?.TotalGrossProfit ?? 0,
                    Rules = rules,
                    PaySlipStatus = (byte)PayslipStatuses.Draft,
                    Deductions = deductions,
                    Allowances = allowances,
                    Shifts = shifts,
                    Commissions = mapperCommissionTables,
                    ProductRevenues = userProductRevenues.Where(x => x.EmployeeId == employee.Id).ToList(),
                    BranchProductRevenues = branchProductRevenues,
                    SettingsToObject = settings,
                    HalfShiftDays = new List<DateTime>()
                };

                var payslip = GetPayslip(engineResource, payslips, employee, rules, isUpdate, payslipCreatedBy, payslipCreatedDate);

                if (payslip == null) return;

                payslip.PayslipClockingPenalizes =
                    (from cp in clockingsPenalizesDto.Where(x => x.EmployeeId == employee.Id).ToList()
                     select new PayslipClockingPenalize
                        {
                            PenalizeId = cp.PenalizeId,
                            ClockingId = cp.ClockingId,
                            TimesCount = cp.TimesCount,
                            MoneyType = cp.MoneyType,
                            Value = cp.Value,
                            PayslipId = payslip.Id,
                            ClockingPenalizeCreated = cp.ClockingPenalizeCreated,
                            ShiftId = cp.ShiftId
                     }).ToList();

                payslip.PayslipPenalizes = 
                    (from cp in payslipClockingPenalizes
                     group cp 
                     by new {cp.PenalizeId, cp.MoneyType}
                     into grCp
                     select new PayslipPenalize(
                         payslip.Id,
                         grCp.Key.PenalizeId,
                         grCp.Sum(x => x.TimesCount * x.Value),
                         grCp.Sum(x => x.TimesCount),
                         grCp.Key.MoneyType,
                         true)).ToList();
                draftPayslips.Add(new DraftPayslipDomainServiceDto
                {
                    Payslip = payslip,
                    Employee = employee,
                });
            });

            return draftPayslips;
        }

        private async Task<List<ProductRevenue>> GetBranchProductRevenues(bool isAllBranch, List<long> branchIds, DateTime from, DateTime to)
        {
            if (isAllBranch)
            {
                var allBranch = (await _kiotVietServiceClient.GetBranch()).Data;
                branchIds = allBranch.Select(x => x.Id).ToList();
            }
            return await _kiotVietServiceClient.GetBranchRevenue(
                _authService.Context.TenantId,
                _authService.Context.TenantCode,
                branchIds,
                from.Date,
                to.Date.AddDays(1).AddTicks(-1)
            );
        }

        private async Task<List<Employee>> GetEmployees(List<Payslip> payslips, List<long> employeeIds, int branchId, bool isUpdate)
        {
            List<Employee> employees;
            if (isUpdate)
            {
                payslips = payslips
                    .Where(p =>
                        p.PayslipStatus != (byte)PayslipStatuses.Void &&
                        p.PayslipStatus != (byte)PayslipStatuses.PaidSalary)
                    .ToList();
                employeeIds = payslips.Select(p => p.EmployeeId).ToList();
                // Cập nhật sẽ lấy cả nhân viên đã xóa
                employees = await _employeeReadOnlyRepository.GetBySpecificationAsync(
                    new FindEmployeeByIdsSpec(employeeIds), false, true);
            }
            else
            {
                ISpecification<Employee> employeeSpec = new FindEmployeeByBranchIdSpec(branchId);
                if (employeeIds != null && employeeIds.Any()) employeeSpec = employeeSpec.And(new FindEmployeeByIdsSpec(employeeIds));
                employees = await _employeeReadOnlyRepository.GetBySpecificationAsync(employeeSpec);
            }

            return employees;
        }

        private Payslip GetPayslip(EngineResource engineResource, List<Payslip> payslips, Employee employee, List<IRule> rules, bool isUpdate, long? payslipCreatedBy, DateTime? payslipCreatedDate)
        {
            Payslip payslip;
            if (isUpdate)
            {
                engineResource.PaySlipStatus = (byte)PayslipStatuses.TemporarySalary;
                payslip = payslips?.FirstOrDefault(p => p.EmployeeId == employee.Id);
                if (payslip != null)
                {
                    engineResource.Bonus = payslip.Bonus;
                    payslip.UpdateWhenDataChanged(rules, engineResource);
                    payslip.PayslipClockings = null;
                }
            }
            else
            {
                payslip = new Payslip(rules, employee.Id, (byte)PayslipStatuses.Draft, engineResource, payslipCreatedBy, payslipCreatedDate, true);
            }

            return payslip;
        }

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
        #endregion
    }
}
