using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeForPaysheet
{
    public class GetEmployeeForPaysheetQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeForPaysheetQuery, PagingDataSource<EmployeeDto>>
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IPayRateDetailReadOnlyRepository _payRateDetailReadOnlyRepository;
        private readonly IPayRateReadOnlyRepository _payRateReadOnlyRepository;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        public GetEmployeeForPaysheetQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IPayRateDetailReadOnlyRepository payRateDetailReadOnlyRepository,
            IPayRateReadOnlyRepository payRateReadOnlyRepository,
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
        ) : base(authService)
        {
            _authService = authService;
            _mapper = mapper;
            _payRateDetailReadOnlyRepository = payRateDetailReadOnlyRepository;
            _payRateReadOnlyRepository = payRateReadOnlyRepository;
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public async Task<PagingDataSource<EmployeeDto>> Handle(GetEmployeeForPaysheetQuery request, CancellationToken cancellationToken)
        {
            if (request.StartTime == default(DateTime) && request.EndTime == default(DateTime))
                throw new KvTimeSheetException("Không xác định thời gian của bảng lương");
            // Lấy danh sách các mức lương thuộc kì lương của bảng lương
            var payRates = await _payRateReadOnlyRepository.GetBySpecificationAsync(new GetPayRateBySalaryPeriodSpec(request.SalaryPeriod));

            var tenantId = _authService.Context.TenantId;

            // Lấy danh sách giảm trừ 
            var deductions = await _deductionReadOnlyRepository.GetBySpecificationAsync(new FindDeductionByTenantIdSpec(tenantId));

            // Lấy danh sách phụ cấp 
            var allowances = await _allowanceReadOnlyRepository.GetBySpecificationAsync(new FindAllowanceByTenantIdSpec(tenantId));

            // Lấy danh sách mức lương chi tiết 
            var payRateDetails =
                await _payRateDetailReadOnlyRepository.GetBySpecificationAsync(
                    new GetPayRateDetailByPayRateIdsSpec(payRates.Select(p => p.Id).ToList()));

            // Loại bỏ payrate detail trong trường hợp nhân viên không thiết lập mức lương nào và mặc định giảm trừ là rỗng
            payRateDetails = payRateDetails.Where(x =>
                    !(x.RuleType == typeof(DeductionRule).Name &&
                    ((JProperty)((JContainer)JsonConvert.DeserializeObject(x.RuleValue)).First).Value.First == null))
                .ToList();

            // Lấy ra danh sách payrate chỉ chứa phụ cấp hoặc giảm trừ
            var payRateAllowanceOrDeductionDetails = payRateDetails.Where(x => x.RuleType == typeof(DeductionRule).Name || x.RuleType == typeof(AllowanceRule).Name).ToList();

            // Kiểm tra nếu tât cả giảm trừ, phụ cấp đã bị xóa thì sẽ không load ra nhân viên này
            foreach (var payRate in payRateAllowanceOrDeductionDetails)
            {
                if (payRate.RuleType == typeof(AllowanceRule).Name && !IsHaveAllowances(payRate, allowances))
                {
                    payRateDetails.Remove(payRate);
                    continue;
                }

                if (payRate.RuleType == typeof(DeductionRule).Name && !IsHaveDeduction(payRate, deductions))
                {
                    payRateDetails.Remove(payRate);
                }
            }

            // Lấy danh sách có áp dụng ít nhất 1 mức lương
            payRates = payRates.Where(p => payRateDetails.Any(pd => pd.PayRateId == p.Id)).ToList();

            // Lấy danh sách id của nhân viên có cài mức lương thuộc kì lương của bảng lương
            var employeeIds = payRates.Select(p => p.EmployeeId).Distinct().ToList();

            // check quyền ko cho phép xem tính lương của nv khác
            var user = AuthService.Context.User;
            var employee = await _employeeReadOnlyRepository.GetByUserIdAsync(user.Id, false, false);
            var isEmployeeLimit = await _authService.HasPermissions(new[] { TimeSheetPermission.EmployeeLimit_Read });

            if (!user.IsAdmin && isEmployeeLimit && employee != null)
            {
                employeeIds = employeeIds.Where(x => x == employee.Id).ToList();
            }

            // Lấy danh sách nhân viên thõa mãn
            var employees = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByIdsSpec(employeeIds).And(new FindEmployeeByBranchIdSpec(request.BranchId)), true);
            employees = employees
                .Where(e => string.IsNullOrEmpty(request.KeyWord) || e.Name.ToLower().Contains(request.KeyWord) || e.Code.ToLower().Contains(request.KeyWord) || e.MobilePhone.ToLower().Contains(request.KeyWord))
                .OrderByDescending(e => e.Id).ToList();

            return new PagingDataSource<EmployeeDto>()
            {
                Data = _mapper.Map<List<Employee>, List<EmployeeDto>>(employees),
                Total = employees.Count
            };
        }

        private bool IsHaveAllowances(PayRateDetail payRateDetail, List<Allowance> allowances)
        {
            var isHaveAllowances = false;
            var existingAllowanceParam = (AllowanceRuleValue)JsonConvert.DeserializeObject(payRateDetail.RuleValue, typeof(AllowanceRuleValue));
            if (existingAllowanceParam != null)
                isHaveAllowances = existingAllowanceParam.AllowanceRuleValueDetails.Any(x =>
                    allowances.Select(a => a.Id).Contains(x.AllowanceId));
            return isHaveAllowances;
        }

        private bool IsHaveDeduction(PayRateDetail payRateDetail, List<Deduction> deductions)
        {
            var isHaveDeductions = false;
            var existingDeductionParam = (DeductionRuleValue)JsonConvert.DeserializeObject(payRateDetail.RuleValue, typeof(DeductionRuleValue));
            if (existingDeductionParam != null)
                isHaveDeductions = existingDeductionParam.DeductionRuleValueDetails.Any(x =>
                    deductions.Select(a => a.Id).Contains(x.DeductionId));
            return isHaveDeductions;
        }
    }
}
