using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetDraftPaysheet
{
    public class GetDraftPaysheetQueryHandler : QueryHandlerBase,
        IRequestHandler<GetDraftPaysheetQuery, PaysheetDto>
    {
        private readonly IWorkingDayForPaysheetDomainService _workingDayForPaysheetDomainService;
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IPenalizeReadOnlyRepository _penalizeReadOnlyRepository;
        private readonly IInitDraftPayslipsDomainService _initDraftPayslipsDomainService;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;

        public GetDraftPaysheetQueryHandler(
            IWorkingDayForPaysheetDomainService workingDayForPaysheetDomainService,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IInitDraftPayslipsDomainService initDraftPayslipsDomainService,
            IPenalizeReadOnlyRepository penalizeReadOnlyRepository,
            IAuthService authService,
            IMapper mapper,
            IMediator mediator,
            IShiftReadOnlyRepository shiftReadOnlyRepository
        ) : base(authService)
        {
            _workingDayForPaysheetDomainService = workingDayForPaysheetDomainService;
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _initDraftPayslipsDomainService = initDraftPayslipsDomainService;
            _mapper = mapper;
            _mediator = mediator;
            _authService = authService;
            _penalizeReadOnlyRepository = penalizeReadOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
        }

        public async Task<PaysheetDto> Handle(GetDraftPaysheetQuery request, CancellationToken cancellationToken)
        {
            var workingDayNumber = request.WorkingDayNumber;
            var branchId = request.BranchId;
            var from = request.StartTime;
            var to = request.EndTime;
            var salaryPeriod = request.SalaryPeriod;
            var paysheetId = request.PaysheetId;
            var employeeIds = request.EmployeeIds;
            var settingObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId));
            var settingToObject = _mapper.Map<SettingsToObject>(settingObjectDto);
            // Lấy số ngày công chuẩn
            var standardWorkingDayNumber = workingDayNumber ?? await _workingDayForPaysheetDomainService.GetWorkingDayPaysheetAsync(branchId, @from, to);
            // Lấy thời gian ngày công chuẩn trong thiết lập cửa hàng
            var timeOfStandardWorkingDay = settingObjectDto.StandardWorkingDay;

            var paysheetDto = new PaysheetDto
            {
                Name = "Bảng lương " + from.ToString("dd/MM/yyyy") + " - " + to.ToString("dd/MM/yyyy"),
                WorkingDayNumber = standardWorkingDayNumber,
                SalaryPeriod = salaryPeriod,
                StartTime = from,
                EndTime = to,
                PaysheetStatus = (byte)(PaysheetStatuses.TemporarySalary),
                TimeOfStandardWorkingDay = timeOfStandardWorkingDay
            };
            var oldPayslips = new List<Payslip>();
            if (paysheetId > 0)
            {
                oldPayslips = await _payslipReadOnlyRepository.GetBySpecificationAsync(new FindPayslipByPaysheetIdSpec(paysheetId).And(new FindPayslipByEmployeeIds(employeeIds)).And(
                            new FindPayslipByStatusSpec((byte)PaysheetStatuses.TemporarySalary)));
            }

            var draftPayslips = await _initDraftPayslipsDomainService.InitDraftPayslipsAsync(null, employeeIds, from,
                to, salaryPeriod, standardWorkingDayNumber, timeOfStandardWorkingDay, branchId, null, null, settingToObject);
            var payslipDtos = new List<PayslipDto>();
            //Lấy vi phạm
            var penalizeLists = draftPayslips.Where(p => p.Payslip.PayslipPenalizes != null).SelectMany(p => p.Payslip.PayslipPenalizes).ToList();
            var penalizeIds = penalizeLists.GroupBy(x => x.PenalizeId).Select(x => x.Key).ToList();
            var penalizeList = await _penalizeReadOnlyRepository.GetBySpecificationAsync(new FindPenalizeByIdsSpec(penalizeIds), false, true);
            //lấy ca theo chi nhánh làm việc
            var branchWorkingIds = draftPayslips.Where(d => d.Employee?.EmployeeBranches != null)
                .SelectMany(d => d.Employee.EmployeeBranches.Select(e => e.BranchId)).ToList();
            var shifts =
                await _shiftReadOnlyRepository.GetShiftMultipleBranchOrderByFromAndTo(
                    branchWorkingIds, new List<long>(), null, true);

            draftPayslips.ForEach(d =>
            {

                var payslipDto = _mapper.Map<PayslipDto>(d.Payslip);
                payslipDto.PayslipStatus = (byte)PaysheetStatuses.TemporarySalary;
                payslipDto.Employee = _mapper.Map<EmployeeDto>(d.Employee);
                payslipDto.Employee.Clockings = _mapper.Map<List<ClockingDto>>(d.Employee.Clockings);
                if (oldPayslips.Any())
                {
                    // Khôi phục giữ liệu của  phiêu lương mới mà mới chỉ đc xóa trên client
                    var oldPayslipEmployee = oldPayslips.Where(p => p.EmployeeId == payslipDto.EmployeeId).OrderByDescending(p => p.CreatedDate).FirstOrDefault();
                    if (oldPayslipEmployee != null)
                    {
                        payslipDto.Id = oldPayslipEmployee.Id;
                        payslipDto.Code = oldPayslipEmployee.Code;
                        payslipDto.TotalPayment = oldPayslipEmployee.TotalPayment;
                        payslipDto.PaysheetId = oldPayslipEmployee.PaysheetId;
                    }

                }
                payslipDto.PayslipPenalizes = GetAndSetNamePayslipPenalizeDto(payslipDto.PayslipPenalizes, penalizeList);
                payslipDto.PayslipClockingPenalizes = GetAndSetNamePayslipClockingPenalizeDto(payslipDto.PayslipClockingPenalizes, shifts);

                payslipDtos.Add(payslipDto);
            });
            
            paysheetDto.Payslips = payslipDtos;
            return paysheetDto;
        }
    }
}
