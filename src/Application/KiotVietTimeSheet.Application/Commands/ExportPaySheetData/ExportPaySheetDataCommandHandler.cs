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
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.ExportPaySheetData
{
    public class ExportPaySheetDataCommandHandler : BaseCommandHandler,
        IRequestHandler<ExportPaySheetDataCommand, PagingDataSource<PaysheetDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IDepartmentReadOnlyRepository _departmentReadOnlyRepository;
        private readonly IJobTitleReadOnlyRepository _jobTitleReadOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly IPayslipPenalizeReadOnlyRepository _payslipPenalizeReadOnlyRepository;
        private readonly IPenalizeReadOnlyRepository _penalizeReadOnlyRepository;

        public ExportPaySheetDataCommandHandler(
            IMapper mapper,
            IEventDispatcher eventDispatcher,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            IDepartmentReadOnlyRepository departmentReadOnlyRepository,
            IJobTitleReadOnlyRepository jobTitleReadOnlyRepository,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IPayslipPenalizeReadOnlyRepository payslipPenalizeReadOnlyRepository,
            IPenalizeReadOnlyRepository penalizeReadOnlyRepository
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
            _payslipPenalizeReadOnlyRepository = payslipPenalizeReadOnlyRepository;
            _penalizeReadOnlyRepository = penalizeReadOnlyRepository;
        }

        public async Task<PagingDataSource<PaysheetDto>> Handle(ExportPaySheetDataCommand request, CancellationToken cancellationToken)
        {
            var isExistFilterExportPost = request.PaysheetByFilter.OrderByDesc == null && request.PaysheetByFilter.OrderBy == null;
            if (isExistFilterExportPost) request.PaysheetByFilter.OrderByDesc = new[] { nameof(Paysheet.CreatedDate) };
            var result = await GetPaySheetsByFilterAsync(request.PaysheetByFilter, true);
            if (result.Data == null || !result.Data.Any()) return result;

            var listEmployeeIds = result.Data.SelectMany(x => x.Payslips).Select(x => x.EmployeeId).Distinct().ToList();
            var listEmployees = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByIdsSpec(listEmployeeIds), false, true);

            var payslipPaymentAllocations = await _kiotVietServiceClient.GetPayslipPaymentsValueIncludeAllocation(
                new GetPayslipPaymentsValueIncludeAllocationReq
                {
                    EmployeeIds = listEmployeeIds
                });

            var listDepartmentIds = listEmployees.Select(x => x.DepartmentId).ToList();
            var departments = await _departmentReadOnlyRepository.GetBySpecificationAsync(new GetDepartmentByIdsSpec(listDepartmentIds), false, true);

            var jobTitleIds = listEmployees.Select(x => x.JobTitleId).ToList();
            var jobTitles = await _jobTitleReadOnlyRepository.GetBySpecificationAsync(new GetJobTitleByIdsSpec(jobTitleIds), false, true);

            var listAllowanceIds = result.Data.SelectMany(s => s.Payslips)
                .Where(payslip => payslip.AllowanceRuleParam != null)
                .SelectMany(payslip => payslip.AllowanceRuleParam.Allowances)
                .Select(allowance => allowance.AllowanceId).Distinct().ToList();
            var deductionIds = result.Data.SelectMany(s => s.Payslips)
                .Where(payslip => payslip.DeductionRuleParam != null)
                .SelectMany(payslip => payslip.DeductionRuleParam.Deductions)
                .Select(deduction => deduction.DeductionId).Distinct().ToList();
            object[] allowanceIdsArray = { listAllowanceIds };
            object[] deductionIdsArray = { deductionIds };

            var allowances = await _allowanceReadOnlyRepository.FindByIdsAsync(allowanceIdsArray, false, true);
            var deductions = await _deductionReadOnlyRepository.FindByIdsAsync(deductionIdsArray, false, true);

            //lay danh sach vi pham
            var payslipPenalizes = await SetDeductionIdToPenalizes(deductions, result.Data);

            foreach (var item in result.Data)
            {
                if (item.Payslips == null || !item.Payslips.Any()) continue;

                item.Payslips = GeneratePayslipTotalPaymentAndEmployeeForExportPaySheet(
                    item.Payslips, payslipPaymentAllocations,
                    listEmployees, departments, jobTitles,
                    allowances, deductions, payslipPenalizes);
            }
            return result;
        }

        #region PRIVATE
        private List<PayslipDto> GeneratePayslipTotalPaymentAndEmployeeForExportPaySheet(
            List<PayslipDto> payslipList,
            List<PayslipPaymentAllocationDto> payslipPaymentAllocations,
            List<Employee> employees,
            List<Department> departments,
            List<JobTitle> jobTitles,
            List<Allowance> allowances,
            List<Deduction> deductions,
            List<PayslipPenalizeDto> payslipPenalizeDtos
        )
        {
            foreach (var payslip in payslipList)
            {
                GeneratePaySlipData(payslip, payslipPaymentAllocations, allowances, deductions, payslipPenalizeDtos);

                var employee = employees.FirstOrDefault(x => x.Id == payslip.EmployeeId);
                payslip.Employee = _mapper.Map<EmployeeDto>(employee);

                if (payslip.Employee == null) continue;

                payslip.Employee.Department = departments.FirstOrDefault(x => x.Id == payslip.Employee.DepartmentId);
                payslip.Employee.JobTitle = jobTitles.FirstOrDefault(x => x.Id == payslip.Employee.JobTitleId);
            }
            return payslipList;
        }

        private void GeneratePaySlipData(PayslipDto payslip,
            List<PayslipPaymentAllocationDto> payslipPaymentAllocations,
            List<Allowance> allowances,
            List<Deduction> deductions,
            List<PayslipPenalizeDto> payslipPenalizeDtos)
        {
            if (payslip.AllowanceRuleParam?.Allowances != null)
            {
                var allowanceParams =
                    payslip.AllowanceRuleParam?.Allowances?
                    .Where(x => x.SelectedItem != false && x.IsChecked != false)
                    .Select(item =>
                    {
                        var allowance = allowances.FirstOrDefault(x => x.Id == item.AllowanceId);
                        item.Name = allowance?.Name ?? item.Name;
                        return item;
                    }).ToList();

                payslip.AllowanceRuleParam.Allowances = allowanceParams;
            }

            if (payslip.DeductionRuleParam?.Deductions != null)
            {
                var deductionParams =
                    payslip.DeductionRuleParam?.Deductions
                        .Where(x => x.SelectedItem != false)
                        .Select(deductionItem =>
                        {
                            var deduction = deductions.FirstOrDefault(x => x.Id == deductionItem.DeductionId);
                            deductionItem.Name = deduction?.Name ?? deductionItem.Name;

                            return deductionItem;
                        }).ToList();
                if(deductionParams.Any()) {
                    payslip.DeductionRuleParam.Deductions = deductionParams;
                }

            }

            if (payslipPenalizeDtos != null) payslip = MergePenalizeToDeduction(payslip, payslipPenalizeDtos);

            var totalPayment = payslip.TotalPayment;
            if (payslip.PayslipStatus == (byte)PaysheetStatuses.PaidSalary) return;

            if (payslipPaymentAllocations == null || payslipPaymentAllocations.Count <= 0) return;

            var employeeAmount =
                payslipPaymentAllocations.FirstOrDefault(x => x.EmployeeId == payslip.EmployeeId);
            totalPayment += employeeAmount?.Amount ?? 0;

            payslip.TotalPayment = totalPayment;
        }

        private async Task<PagingDataSource<PaysheetDto>> GetPaySheetsByFilterAsync(PaySheetQueryFilter filter, bool includePaySlips = false)
        {
            var result = await _paysheetReadOnlyRepository.GetListByQueryFilterAsync(filter, includePaySlips);
            var dicPaySheetEmployeeIds = result.dicPaysheetEmployeeIds;

            // Lấy giá trị phân bổ để tính thêm vào tổng thanh toán cho bảng lương tạm tính
            if (dicPaySheetEmployeeIds.Any())
            {
                var employeeIds = dicPaySheetEmployeeIds.Values.SelectMany(s => s).Distinct().ToList();
                var employeeWithAllocate = await _kiotVietServiceClient.GetPayslipPaymentsValueIncludeAllocation(
                    new GetPayslipPaymentsValueIncludeAllocationReq
                    {
                        EmployeeIds = employeeIds
                    });

                if (employeeWithAllocate.Any())
                {
                    result.totalPayment += employeeWithAllocate.Sum(s => s.Amount);

                    result.dataSource.Data = GeneratePaySheetTotalNeedPay(result.dataSource.Data, dicPaySheetEmployeeIds, employeeWithAllocate);
                }
            }

            var pgTotalNeedPay = result.totalNetSalary - result.totalPayment;
            if (pgTotalNeedPay <= 0) pgTotalNeedPay = 0;
            return new PaysheetPagingDataSource
            {
                Total = result.dataSource.Total,
                Data = _mapper.Map<List<PaysheetDto>>(result.dataSource.Data),
                TotalNetSalary = result.totalNetSalary,
                TotalPayment = result.totalPayment,
                TotalNeedPay = pgTotalNeedPay
            };
        }

        private IList<Paysheet> GeneratePaySheetTotalNeedPay(IList<Paysheet> paySheetList, Dictionary<long, List<long>> dicPaySheetEmployeeIds, List<PayslipPaymentAllocationDto> employeeWithAllocate)
        {
            foreach (var paySheet in paySheetList)
            {
                if (!dicPaySheetEmployeeIds.ContainsKey(paySheet.Id) ||
                    paySheet.PaysheetStatus == (byte)PaysheetStatuses.PaidSalary) continue;
                paySheet.TotalPayment += employeeWithAllocate.Where(p =>
                        dicPaySheetEmployeeIds[paySheet.Id].Contains(p.EmployeeId))
                    .Sum(p => p.Amount);

                var totalNeedPay = paySheet.TotalNetSalary - paySheet.TotalPayment;
                if (totalNeedPay <= 0) totalNeedPay = 0;
                paySheet.TotalNeedPay = totalNeedPay;
            }

            return paySheetList;
        }

        private PayslipDto MergePenalizeToDeduction(PayslipDto payslip, IEnumerable<PayslipPenalizeDto> payslipPenalizes)
        {
            var deductionRuleParam = new DeductionRuleParam();
            if (payslip.DeductionRuleParam == null) payslip.DeductionRuleParam = deductionRuleParam;
            var payslipPenalizeDeductions = new List<DeductionParam>();

            foreach (var payslipPenalizeDto in payslipPenalizes)
            {
                if (payslip.Id != payslipPenalizeDto.PayslipId) continue;
                var payslipPenalizeDeductionParam = new DeductionParam
                {
                    Value = payslipPenalizeDto.Value,
                    Name = payslipPenalizeDto.PenalizeName,
                    DeductionId = payslipPenalizeDto.DeductionId
                };
                payslipPenalizeDeductions.Add(payslipPenalizeDeductionParam);
            }

            if (payslip.DeductionRuleParam.Deductions == null)
                payslip.DeductionRuleParam.Deductions = payslipPenalizeDeductions;
            payslip.DeductionRuleParam.Deductions.AddRange(payslipPenalizeDeductions);
            return payslip;
        }

        private async Task<List<PayslipPenalizeDto>> SetDeductionIdToPenalizes(List<Deduction> deductions, IList<PaysheetDto> paysheetDto)
        {
            var maxDeductionId = (long)0;
            if (deductions != null && deductions.Any())
                maxDeductionId = deductions.Max(d => d.Id);

            var payslipIds = paysheetDto.SelectMany(x => x.Payslips).Select(p => p.Id).ToList();
            var payslipPenalizes = await _payslipPenalizeReadOnlyRepository.GetPayslipsPenalizes(payslipIds);
            if (payslipPenalizes == null) return new List<PayslipPenalizeDto>();

            var penalizeIds = payslipPenalizes?.Select(p => p.PenalizeId).Distinct().ToList();
            object[] penalizeIdsArr = { penalizeIds };
            var penalizes = await _penalizeReadOnlyRepository.FindByIdsAsync(penalizeIdsArr, false, true);
            foreach (var penalize in payslipPenalizes)
            {
                var existDeductionIdOnPenalize =
                    payslipPenalizes.FirstOrDefault(p => p.PenalizeId == penalize.PenalizeId && p.DeductionId != 0);
                if (existDeductionIdOnPenalize != null) penalize.DeductionId = existDeductionIdOnPenalize.DeductionId;
                else
                {
                    maxDeductionId++;
                    penalize.DeductionId = maxDeductionId;
                }

                penalize.PenalizeName = penalizes?.FirstOrDefault(p => p.Id == penalize.PenalizeId)?.Name;
            }

            return payslipPenalizes;
        }
        #endregion
    }
}
