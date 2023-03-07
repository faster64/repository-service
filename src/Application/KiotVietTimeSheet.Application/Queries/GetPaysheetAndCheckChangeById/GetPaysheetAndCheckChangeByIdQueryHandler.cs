using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetAndCheckChangeById
{
    public class GetPaysheetAndCheckChangeByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPaysheetAndCheckChangeByIdQuery, PaysheetDto>
    {
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IEmployeeBranchReadOnlyRepository _employeeBranchReadOnlyRepository;
        private readonly IPenalizeReadOnlyRepository _penalizeReadOnlyRepository;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;

        public GetPaysheetAndCheckChangeByIdQueryHandler(
            IAuthService authService,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IMapper mapper,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IEmployeeBranchReadOnlyRepository employeeBranchReadOnlyRepository,
            IPenalizeReadOnlyRepository penalizeReadOnlyRepository,
            IEventDispatcher eventDispatcher,
            IShiftReadOnlyRepository shiftReadOnlyRepository
        ) : base(authService)
        {
            _mapper = mapper;
            _authService = authService;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _employeeBranchReadOnlyRepository = employeeBranchReadOnlyRepository;
            _penalizeReadOnlyRepository = penalizeReadOnlyRepository;
            _eventDispatcher = eventDispatcher;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
        }

        public async Task<PaysheetDto> Handle(GetPaysheetAndCheckChangeByIdQuery request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var branchId = request.BranchId;
            var kvSessionBranchId = request.KvSessionBranchId;

            var paySheet = await _paysheetReadOnlyRepository.FindBySpecificationAsync(
                new FindPaysheetByIdSpec(id).And(new FindPaysheetByBranchId(branchId)).And(
                    new FindPaysheetByStatuses(new List<byte> { (byte)PaysheetStatuses.TemporarySalary, (byte)PaysheetStatuses.Draft })));
            if (paySheet == null)
            {
                NotifyPaysheetInDbIsNotExists();
                return null;
            }
            // Chuyển chi nhánh sẽ trở về trang ds tính lương
            if (kvSessionBranchId != _authService.Context.BranchId)
            {
                return _mapper.Map<PaysheetDto>(paySheet);
            }

            // Lấy thời gian ngày công chuẩn trong thiết lập cửa hàng
            var paySheetDto = _mapper.Map<PaysheetDto>(paySheet);
            var payslipStatus = paySheet.IsDraft ? (byte)PayslipStatuses.Draft : (byte)PayslipStatuses.TemporarySalary;
            var payslipSpec = new GetPayslipByPaysheetId(id).And(new FindPayslipByStatusSpec(payslipStatus));
            // check quyền ko cho phép xem tính lương của nv khác
            var user = AuthService.Context.User;
            var employee = await _employeeReadOnlyRepository.GetByUserIdAsync(user.Id, false, false);
            var isEmployeeLimit = await _authService.HasPermissions(new[] { TimeSheetPermission.EmployeeLimit_Read });

            if (!user.IsAdmin && isEmployeeLimit && employee != null)
            {
                payslipSpec = payslipSpec.And(new FindPayslipByEmployeeId(employee.Id));
            }
            var payslips = await _payslipReadOnlyRepository.GetBySpecificationAsync(payslipSpec, true);
            var employeeIds = payslips.Select(p => p.EmployeeId).ToList();
            var employees = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByIdsSpec(employeeIds), false, true);

            //Lấy danh sách chi nhánh làm việc của nhân viên
            var employeesDto = _mapper.Map<List<EmployeeDto>>(employees);
            var lstBranchWorking =
                await _employeeBranchReadOnlyRepository.GetBySpecificationAsync(
                    new FindBranchByEmployeeIdsSpec(employeeIds));
            employees.ForEach(e =>
            {
                var lstBranchWithEmployee = lstBranchWorking.Where(x => x.EmployeeId == e.Id).ToList();
                e.EmployeeBranches = lstBranchWithEmployee;
            });

            var clockingUnAuthorizeAbsences = await _clockingReadOnlyRepository.GetClockingUnAuthorizedAbsence(paySheetDto.StartTime, paySheetDto.EndTime, employeeIds);
            
            var penalizeIds =
                    payslips.Where(x => x.PayslipPenalizes != null)
                            .SelectMany(x => x.PayslipPenalizes)
                            .GroupBy(x => x.PenalizeId)
                            .Select(x => x.Key)
                            .ToList();

            var penalizes = await _penalizeReadOnlyRepository.GetBySpecificationAsync(new FindPenalizeByIdsSpec(penalizeIds), false, true);

            var shifts =
                await _shiftReadOnlyRepository.GetShiftMultipleBranchOrderByFromAndTo(
                    lstBranchWorking.Select(x => x.BranchId).ToList(), new List<long>(), null, true);

            var payslipsDto = new List<PayslipDto>();
            payslips.ForEach(payslip =>
            {
                var payslipDto = _mapper.Map<PayslipDto>(payslip);
                payslipDto.Employee = employeesDto.FirstOrDefault(e => e.Id == payslip.EmployeeId);
                payslipDto.Code = payslipDto.PayslipStatus == (byte)PayslipStatuses.Draft ? string.Empty : payslipDto.Code;
                payslipDto.PayslipStatus = (byte)PayslipStatuses.TemporarySalary;
                payslipDto.PayslipClockings = _mapper.Map<List<PayslipClockingDto>>(payslip.PayslipClockings);
                if (payslipDto.PayslipClockings != null)
                    payslipDto.AuthorisedAbsence = payslipDto.PayslipClockings.GroupBy(x => x.StartTime.Date).Select(
                        x =>
                            x.Any(y => y.AbsenceType != (byte) AbsenceTypes.AuthorisedAbsence) ? 0 : 1).Sum();
                
                var clockingUnAuthorizeAbsenceItem = clockingUnAuthorizeAbsences?.FirstOrDefault(c => c.Key == payslip.EmployeeId);

                payslipDto.PayslipClockingPenalizes = GetAndSetNamePayslipClockingPenalizeDto(payslipDto.PayslipClockingPenalizes, shifts);

                payslipDto.PayslipPenalizes = GetAndSetNamePayslipPenalizeDto(payslipDto.PayslipPenalizes, penalizes);

                payslipDto.UnauthorisedAbsence = clockingUnAuthorizeAbsenceItem?.Value ?? 0;

                payslipsDto.Add(payslipDto);
            });
            paySheetDto.Payslips = payslipsDto;

            return paySheetDto;
        }

        #region PRIVATE
        private void NotifyPaysheetInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Paysheet).Name, string.Format(Message.not_exists, Label.paysheet)));
        }
        #endregion
    }
}
