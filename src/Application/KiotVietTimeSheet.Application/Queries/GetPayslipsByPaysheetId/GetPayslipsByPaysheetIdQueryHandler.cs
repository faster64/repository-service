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
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipsByPaysheetId
{
    public class GetPayslipsByPaysheetIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayslipsByPaysheetIdQuery, PagingDataSource<PayslipDto>>
    {
        private readonly IMapper _mapper;
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IEmployeeBranchReadOnlyRepository _employeeBranchReadOnlyRepository;
        private readonly IPenalizeReadOnlyRepository _penalizeReadOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IMediator _mediator;
        public GetPayslipsByPaysheetIdQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IEmployeeBranchReadOnlyRepository employeeBranchReadOnlyRepository,
            IPenalizeReadOnlyRepository penalizeReadOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IMediator mediator
        ) : base(authService)
        {
            _mapper = mapper;
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _employeeBranchReadOnlyRepository = employeeBranchReadOnlyRepository;
            _penalizeReadOnlyRepository = penalizeReadOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _mediator = mediator;
        }

        public async Task<PagingDataSource<PayslipDto>> Handle(GetPayslipsByPaysheetIdQuery request, CancellationToken cancellationToken)
        {
            var ds = await _payslipReadOnlyRepository.GetByQueryFilterWithSummary(request.Filter);

            var employeeIds = ds.dataSource.Data.Select(e => e.EmployeeId).Distinct().ToList();

            var paySheet = await _paysheetReadOnlyRepository.FindByIdAsync(request.Filter.PaysheetId);

            var clockingUnAuthorizeAbsences =
                await _clockingReadOnlyRepository.GetClockingUnAuthorizedAbsence(paySheet.StartTime, paySheet.EndTime, employeeIds);

            var settingObjectDto = await _mediator.Send(new GetSettingQuery(paySheet.TenantId), cancellationToken);

            var pgDataSource = new PayslipPagingDataSource
            {
                Total = ds.dataSource.Total,
                Data = _mapper.Map<List<PayslipDto>>(ds.dataSource.Data),
                TotalMainSalary = ds.totalMainSalary,
                TotalCommissionSalary = ds.totalCommissionSalary,
                TotalOvertimeSalary = ds.totalOvertimeSalary,
                TotalAllowance = ds.totalAllowance,
                TotalBonus = ds.totalBonus,
                TotalDeduction = ds.totalDeduction,
                TotalNetSalary = ds.totalNetSalary,
                TotalPayment = ds.totalPayment
            };

            pgDataSource.Data = GeneratePayslip(pgDataSource.Data, clockingUnAuthorizeAbsences, settingObjectDto);

            if (pgDataSource.Data == null || !pgDataSource.Data.Any()) return pgDataSource;

            var isTemporaryPaySheet = pgDataSource.Data.First().PayslipStatus == (byte)PayslipStatuses.TemporarySalary;

            // Lấy giá trị phân bổ để tính thêm vào tổng thanh toán cho phiếu lương tạm tính
            var dicPayslipEmployeeIdItem = ds.dicPayslipEmployeeId;
            if (dicPayslipEmployeeIdItem != null && dicPayslipEmployeeIdItem.Any())
            {
                pgDataSource = await GeneratePayslipTotalPayment(employeeIds, isTemporaryPaySheet, pgDataSource, dicPayslipEmployeeIdItem);
            }

            var listEmployee = await _employeeReadOnlyRepository.GetBySpecificationAsync(
                new FindEmployeeByIdsSpec(pgDataSource.Data.Select(r => r.EmployeeId).ToList()), false, true);

            //Lấy danh sách chi nhánh làm việc của nhân viên
            var lstBranchWorking =
                await _employeeBranchReadOnlyRepository.GetBySpecificationAsync(
                    new FindBranchByEmployeeIdsSpec(listEmployee.Select(e => e.Id).ToList()));

            //lay danh sach vi pham
            var penalizeLists = pgDataSource.Data.Where(p => p.PayslipPenalizes != null).SelectMany(p => p.PayslipPenalizes).ToList();
            var penalizeIds = penalizeLists.GroupBy(x => x.PenalizeId).Select(x => x.Key).ToList();

            var shifts =
                await _shiftReadOnlyRepository.GetShiftMultipleBranchOrderByFromAndTo(
                    lstBranchWorking.Select(x => x.BranchId).ToList(), new List<long>(), null, true);

            var penalizeList = await _penalizeReadOnlyRepository.GetBySpecificationAsync(new FindPenalizeByIdsSpec(penalizeIds), false, true);

            foreach (var payslip in pgDataSource.Data)
            {
                payslip.PayslipPenalizes = GetAndSetNamePayslipPenalizeDto(payslip.PayslipPenalizes, penalizeList);


                payslip.PayslipClockingPenalizes = GetAndSetNamePayslipClockingPenalizeDto(payslip.PayslipClockingPenalizes, shifts);

                var employee = listEmployee.FirstOrDefault(f => f.Id == payslip.EmployeeId);
                if (employee != null)
                {
                    employee.EmployeeBranches = lstBranchWorking.Where(b => b.EmployeeId == employee.Id).ToList();
                }
                payslip.Employee = _mapper.Map<EmployeeDto>(employee);
            }

            //update lại tổng Còn cần trả dựa vào phân bổ nếu các phiếu lương tạm tính
            pgDataSource.TotalNeedPay = 0;
            var needPay = pgDataSource.TotalNetSalary - pgDataSource.TotalPayment;
            if (needPay > 0) pgDataSource.TotalNeedPay = needPay;
            return pgDataSource;
        }

        #region PRIVATE

        private static IList<PayslipDto> GeneratePayslip(IList<PayslipDto> listPayslip, List<KeyValuePair<long, int>> clockingUnAuthorizeAbsences, SettingObjectDto settingObjectDto)
        {
            foreach (var payslipItem in listPayslip)
            {
                payslipItem.AuthorisedAbsence = HaftShiftUtils.CalculateAuthorisedAbsence(payslipItem, settingObjectDto);

                var clockingUnAuthorizeAbsenceItem = clockingUnAuthorizeAbsences?.FirstOrDefault(c => c.Key == payslipItem.EmployeeId);
                payslipItem.UnauthorisedAbsence = clockingUnAuthorizeAbsenceItem?.Value ?? 0;
            }
            return listPayslip;
        }

        private async Task<PayslipPagingDataSource> GeneratePayslipTotalPayment(List<long> listEmployeeIds, bool isTemporaryPaySheet, PayslipPagingDataSource pgDataSource, Dictionary<long, long> dicPayslipEmployeeId)
        {
            var employeeWithAllocate = await _kiotVietServiceClient.GetPayslipPaymentsValueIncludeAllocation(
                new GetPayslipPaymentsValueIncludeAllocationReq
                {
                    EmployeeIds = listEmployeeIds
                });
            var sumAmount = (decimal)0;
            if (employeeWithAllocate != null) sumAmount = employeeWithAllocate.Sum(x => x.Amount);

            if (isTemporaryPaySheet) pgDataSource.TotalPayment += sumAmount;

            foreach (var payslipItem in pgDataSource.Data)
            {
                var hasPayslipId = dicPayslipEmployeeId.Any(x => x.Key == payslipItem.Id);
                if (payslipItem.PayslipStatus != (byte)PayslipStatuses.TemporarySalary || !hasPayslipId) continue;

                var employeeWithAllocateAmount = (decimal)0;
                if (employeeWithAllocate != null)
                {
                    employeeWithAllocateAmount = employeeWithAllocate
                        .Where(p => p.EmployeeId == dicPayslipEmployeeId[payslipItem.Id])
                        .Sum(p => p.Amount);
                }
                payslipItem.TotalPayment += employeeWithAllocateAmount;
            }

            return pgDataSource;
        }
        #endregion
    }
}
