using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPaySlipsByQueryFilter
{
    public class GetPaySlipsByQueryFilterQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPaySlipsByQueryFilterQuery, PagingDataSource<PayslipDto>>
    {
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IPenalizeReadOnlyRepository _penalizeReadOnlyRepository;
        private readonly IEmployeeBranchReadOnlyRepository _employeeBranchReadOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        public GetPaySlipsByQueryFilterQueryHandler(
            IAuthService authService,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IMapper mapper,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IPenalizeReadOnlyRepository penalizeReadOnlyRepository,
            IEmployeeBranchReadOnlyRepository employeeBranchReadOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository

        ) : base(authService)
        {
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _mapper = mapper;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _penalizeReadOnlyRepository = penalizeReadOnlyRepository;
            _employeeBranchReadOnlyRepository = employeeBranchReadOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
        }

        public async Task<PagingDataSource<PayslipDto>> Handle(GetPaySlipsByQueryFilterQuery request, CancellationToken cancellationToken)
        {
            var ds = await _payslipReadOnlyRepository.GetByQueryFilter(request.Filter);
            var paySheets = await _paysheetReadOnlyRepository.GetBySpecificationAsync(
                new FindPaysheetByIdsSpec(ds.Data.Select(s => s.PaysheetId).ToList()));

            var employees = await _employeeReadOnlyRepository.GetBySpecificationAsync(
                new FindEmployeeByIdsSpec(ds.Data.Select(r => r.EmployeeId).ToList()));

            var payslipIds = ds.Data.Select(x => x.Id).ToList();
            var employeeId = request.Filter.EmployeeId ?? 0;
            var clockingsUnauthorisedAbsence =
                await _clockingReadOnlyRepository.GetClockingUnAuthorizeAbsenceByPaySheetIds(paySheets.Select(x => x.Id).ToList(), payslipIds, employeeId);

            var isClockingsUnauthorisedAbsence = clockingsUnauthorisedAbsence != null && clockingsUnauthorisedAbsence.Any();

            var penalizeLists = ds.Data.Where(p => p.PayslipPenalizes != null).SelectMany(p => p.PayslipPenalizes).ToList();
            var penalizeIds = penalizeLists.Select(x => x.PenalizeId).ToList();
            var penalizeList = await _penalizeReadOnlyRepository.GetBySpecificationAsync(new FindPenalizeByIdsSpec(penalizeIds), false, true);

            var lstBranchWorking =
                await _employeeBranchReadOnlyRepository.GetBySpecificationAsync(
                    new FindBranchByEmployeeIdsSpec(ds.Data.Select(r => r.EmployeeId).ToList()));

            var branchWorkingIds = lstBranchWorking.GroupBy(x => x.BranchId).Select(x => x.Key).ToList();

            var shifts =
                await _shiftReadOnlyRepository.GetShiftMultipleBranchOrderByFromAndTo(
                    branchWorkingIds, new List<long>(), null, true);

            var result = ds.Data.Select(payslip =>
            {
                var payslipDto = _mapper.Map<PayslipDto>(payslip);
                var employee = employees.FirstOrDefault(f => f.Id == payslip.EmployeeId);
                var paySheet = paySheets.FirstOrDefault(f => f.Id == payslip.PaysheetId);

                payslipDto.PayslipPenalizes = GetAndSetNamePayslipPenalizeDto(payslipDto.PayslipPenalizes, penalizeList);

                payslipDto.PayslipClockingPenalizes = GetAndSetNamePayslipClockingPenalizeDto(payslipDto.PayslipClockingPenalizes, shifts);

                payslipDto.Employee = _mapper.Map<EmployeeDto>(employee);
                payslipDto.Paysheet = _mapper.Map<PaysheetDto>(paySheet);

                #region Tính ngày nghỉ có phép
                var countAbsence = 0;
                var dicPayslipLsClocking =
                    payslipDto.PayslipClockings?
                        .GroupBy(x => x.StartTime.Date)
                        .Select(x => new
                        {
                            dateKey = x.Key,
                            payslipClokings = x.Select(g => g)
                        }).ToList();

                dicPayslipLsClocking?.ForEach(x =>
                {
                    var isOtherAbsence =
                        x.payslipClokings.Any(pc => pc.AbsenceType != (byte)AbsenceTypes.AuthorisedAbsence);

                    countAbsence += isOtherAbsence ? 0 : 1;
                });

                payslipDto.AuthorisedAbsence = countAbsence;
                #endregion

                if (isClockingsUnauthorisedAbsence)
                {
                    var clockingUnauthorisedAbsenceItem = clockingsUnauthorisedAbsence?.FirstOrDefault(c => c.Key == payslip.Id);
                    payslipDto.UnauthorisedAbsence = clockingUnauthorisedAbsenceItem?.Value ?? 0;
                }
                else
                {
                    payslipDto.UnauthorisedAbsence = 0;
                }

                return payslipDto;
            })
                .ToList();

            // Lấy giá trị phân bổ để tính thêm vào tổng thanh toán cho phiếu lương tạm tính
            if (result.Any())
            {
                var employeeWithAllocate = await _kiotVietServiceClient.GetPayslipPaymentsValueIncludeAllocation(
                    new GetPayslipPaymentsValueIncludeAllocationReq
                    {
                        EmployeeIds = employees.Select(x => x.Id).ToList()
                    });


                if (employeeWithAllocate != null && employeeWithAllocate.Count > 0)
                {
                    result.ForEach(payslip =>
                    {
                        payslip.TotalPayment = CalculatorTotalPayment(payslip, employeeWithAllocate);
                    });
                }
            }

            return new PagingDataSource<PayslipDto>
            {
                Data = result,
                Total = ds.Total
            };
        }

        #region Private method

        private decimal CalculatorTotalPayment(PayslipDto payslip, List<PayslipPaymentAllocationDto> employeeWithAllocate)
        {
            if (payslip.PayslipStatus != (byte)PayslipStatuses.TemporarySalary) return payslip.TotalPayment;

            return payslip.TotalPayment + employeeWithAllocate
                       .Where(p => p.EmployeeId == payslip.EmployeeId)
                       .Sum(p => p.Amount);
        }


        #endregion
    }
}
