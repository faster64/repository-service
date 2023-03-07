using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.ExportPaySheet
{
    public class ExportPaySheetCommandHandler : BaseCommandHandler,
        IRequestHandler<ExportPaySheetCommand, PagingDataSource<PaysheetDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IDepartmentReadOnlyRepository _departmentReadOnlyRepository;
        private readonly IJobTitleReadOnlyRepository _jobTitleReadOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;

        public ExportPaySheetCommandHandler(
            IMapper mapper,
            IEventDispatcher eventDispatcher,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            IDepartmentReadOnlyRepository departmentReadOnlyRepository,
            IJobTitleReadOnlyRepository jobTitleReadOnlyRepository,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IDeductionReadOnlyRepository deductionReadOnlyRepository
        ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _departmentReadOnlyRepository = departmentReadOnlyRepository;
            _jobTitleReadOnlyRepository = jobTitleReadOnlyRepository;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
        }

        public async Task<PagingDataSource<PaysheetDto>> Handle(ExportPaySheetCommand request, CancellationToken cancellationToken)
        {
            var isExistFilterExportGet = request.PaysheetByFilter.OrderByDesc == null && request.PaysheetByFilter.OrderBy == null;
            if (isExistFilterExportGet) request.PaysheetByFilter.OrderByDesc = new[] { nameof(Paysheet.CreatedDate) };
            var result = await GetPaySheetsInExportGetByFilterAsync(request.PaysheetByFilter, true);
            if (result.Data == null || !result.Data.Any()) return result;

            var employeeIdsInExportGet = result.Data.SelectMany(x => x.Payslips).Select(x => x.EmployeeId).Distinct().ToList();
            var employeesInExportGet = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByIdsSpec(employeeIdsInExportGet), false, true);

            var payslipPaymentAllocations = await _kiotVietServiceClient.GetPayslipPaymentsValueIncludeAllocation(
                new GetPayslipPaymentsValueIncludeAllocationReq
                {
                    EmployeeIds = employeeIdsInExportGet
                });

            var departmentIds = employeesInExportGet.Select(x => x.DepartmentId).ToList();
            var departments = await _departmentReadOnlyRepository.GetBySpecificationAsync(new GetDepartmentByIdsSpec(departmentIds), false, true);

            var jobTitleIds = employeesInExportGet.Select(x => x.JobTitleId).ToList();
            var jobTitles = await _jobTitleReadOnlyRepository.GetBySpecificationAsync(new GetJobTitleByIdsSpec(jobTitleIds), false, true);

            var allowanceIdsInExportGet = result.Data.SelectMany(s => s.Payslips)
                .Where(payslip => payslip.AllowanceRuleParam != null)
                .SelectMany(payslip => payslip.AllowanceRuleParam.Allowances)
                .Select(allowance => allowance.AllowanceId).Distinct().ToList();
            var deductionIdsInExportGet = result.Data.SelectMany(s => s.Payslips)
                .Where(payslip => payslip.DeductionRuleParam != null)
                .SelectMany(payslip => payslip.DeductionRuleParam.Deductions)
                .Select(deduction => deduction.DeductionId).Distinct().ToList();
            object[] allowanceIdsArray = { allowanceIdsInExportGet };
            object[] deductionIdsArray = { deductionIdsInExportGet };

            var allowancesInExportGet = await _allowanceReadOnlyRepository.FindByIdsAsync(allowanceIdsArray, false, true);
            var deductionsInExportGet = await _deductionReadOnlyRepository.FindByIdsAsync(deductionIdsArray, false, true);

            foreach (var item in result.Data)
            {
                if (item.Payslips == null || !item.Payslips.Any()) continue;

                item.Payslips = GeneratePayslipTotalPaymentAndEmployee(
                    item.Payslips, payslipPaymentAllocations,
                    employeesInExportGet, departments, jobTitles,
                    allowancesInExportGet, deductionsInExportGet);
            }
            return result;
        }

        #region PRIVATE
        private List<PayslipDto> GeneratePayslipTotalPaymentAndEmployee(
            List<PayslipDto> payslipList,
            List<PayslipPaymentAllocationDto> payslipPaymentAllocations,
            List<Employee> employees,
            List<Department> departments,
            List<JobTitle> jobTitles,
            List<Allowance> allowances,
            List<Deduction> deductions
        )
        {
            foreach (var payslip in payslipList)
            {
                GeneratePaySlipInExportGet(payslip, payslipPaymentAllocations, allowances, deductions);

                var employee = employees.FirstOrDefault(x => x.Id == payslip.EmployeeId);
                payslip.Employee = _mapper.Map<EmployeeDto>(employee);

                if (payslip.Employee == null) continue;

                payslip.Employee.Department = departments.FirstOrDefault(x => x.Id == payslip.Employee.DepartmentId);
                payslip.Employee.JobTitle = jobTitles.FirstOrDefault(x => x.Id == payslip.Employee.JobTitleId);
            }
            return payslipList;
        }

        private void GeneratePaySlipInExportGet(PayslipDto payslip,
            List<PayslipPaymentAllocationDto> payslipPaymentAllocations,
            List<Allowance> allowances,
            List<Deduction> deductions)
        {
            if (payslip.AllowanceRuleParam?.Allowances != null)
            {
                var allowanceParamsInExportGet =
                    payslip.AllowanceRuleParam?.Allowances?
                    .Where(x => x.SelectedItem != false)
                    .Select(item =>
                    {
                        var allowance = allowances.FirstOrDefault(x => x.Id == item.AllowanceId);
                        item.Name = allowance?.Name;
                        return item;
                    }).ToList();

                payslip.AllowanceRuleParam.Allowances = allowanceParamsInExportGet;
            }

            if (payslip.DeductionRuleParam?.Deductions != null)
            {
                var deductionParamsInExportGet =
                    payslip.DeductionRuleParam?.Deductions
                        .Where(x => x.SelectedItem != false)
                        .Select(deductionItem =>
                        {
                            var deduction = deductions.FirstOrDefault(x => x.Id == deductionItem.DeductionId);
                            deductionItem.Name = deduction?.Name;

                            return deductionItem;
                        }).ToList();

                payslip.DeductionRuleParam.Deductions = deductionParamsInExportGet;
            }

            var totalPayment = payslip.TotalPayment;
            if (payslip.PayslipStatus == (byte)PaysheetStatuses.PaidSalary) return;

            if (payslipPaymentAllocations == null || payslipPaymentAllocations.Count <= 0) return;

            var employeeAmount =
                payslipPaymentAllocations.FirstOrDefault(x => x.EmployeeId == payslip.EmployeeId);
            totalPayment += employeeAmount?.Amount ?? 0;

            payslip.TotalPayment = totalPayment;
        }

        private async Task<PagingDataSource<PaysheetDto>> GetPaySheetsInExportGetByFilterAsync(PaySheetQueryFilter filter, bool includePaySlips = false)
        {
            var resultInExportGet = await _paysheetReadOnlyRepository.GetListByQueryFilterAsync(filter, includePaySlips);
            var dicPaySheetEmployeeIds = resultInExportGet.dicPaysheetEmployeeIds;

            // Lấy giá trị phân bổ để tính thêm vào tổng thanh toán cho bảng lương tạm tính
            if (dicPaySheetEmployeeIds.Any())
            {
                var employeeIdsInExportGet = dicPaySheetEmployeeIds.Values.SelectMany(s => s).Distinct().ToList();
                var employeeWithAllocate = await _kiotVietServiceClient.GetPayslipPaymentsValueIncludeAllocation(
                    new GetPayslipPaymentsValueIncludeAllocationReq
                    {
                        EmployeeIds = employeeIdsInExportGet
                    });

                if (employeeWithAllocate.Any())
                {
                    resultInExportGet.totalPayment += employeeWithAllocate.Sum(s => s.Amount);

                    resultInExportGet.dataSource.Data = GeneratePaySheetTotalNeedPayInExportGet(resultInExportGet.dataSource.Data, dicPaySheetEmployeeIds, employeeWithAllocate);
                }
            }

            var pgTotalNeedPayInExportGet = resultInExportGet.totalNetSalary - resultInExportGet.totalPayment;
            if (pgTotalNeedPayInExportGet <= 0) pgTotalNeedPayInExportGet = 0;
            return new PaysheetPagingDataSource
            {
                Total = resultInExportGet.dataSource.Total,
                Data = _mapper.Map<List<PaysheetDto>>(resultInExportGet.dataSource.Data),
                TotalNetSalary = resultInExportGet.totalNetSalary,
                TotalPayment = resultInExportGet.totalPayment,
                TotalNeedPay = pgTotalNeedPayInExportGet
            };
        }

        private IList<Paysheet> GeneratePaySheetTotalNeedPayInExportGet(IList<Paysheet> paySheetList, Dictionary<long, List<long>> dicPaySheetEmployeeIds, List<PayslipPaymentAllocationDto> employeeWithAllocate)
        {
            foreach (var paySheet in paySheetList)
            {
                if (!dicPaySheetEmployeeIds.ContainsKey(paySheet.Id) ||
                    paySheet.PaysheetStatus == (byte)PaysheetStatuses.PaidSalary) continue;
                paySheet.TotalPayment += employeeWithAllocate.Where(p =>
                        dicPaySheetEmployeeIds[paySheet.Id].Contains(p.EmployeeId))
                    .Sum(p => p.Amount);

                var totalNeedPayInExportGet = paySheet.TotalNetSalary - paySheet.TotalPayment;
                if (totalNeedPayInExportGet <= 0) totalNeedPayInExportGet = 0;
                paySheet.TotalNeedPay = totalNeedPayInExportGet;
            }

            return paySheetList;
        }

        #endregion
    }
}
