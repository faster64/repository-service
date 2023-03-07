using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.ExportPaySheetCommissionDetailData
{
    public class ExportPaySheetCommissionDetailCommandHandler : BaseCommandHandler,
        IRequestHandler<ExportPaySheetCommissionDetailCommand, PaysheetDto>
    {
        #region Properties
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;

        #endregion

        public ExportPaySheetCommissionDetailCommandHandler(
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IMapper mapper,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IEventDispatcher eventDispatcher,
            IMediator mediator,
            IAuthService authService
        ) : base(eventDispatcher)
        {
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _mapper = mapper;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _mediator = mediator;
            _authService = authService;
        }

        public async Task<PaysheetDto> Handle(ExportPaySheetCommissionDetailCommand request, CancellationToken cancellationToken)
        {
            var paySheet = await _paysheetReadOnlyRepository.FindBySpecificationAsync(
                new FindPaysheetByIdSpec(request.PaySheetId)
                    .And(new FindPaysheetByBranchId(request.BranchId))
            );

            if (paySheet == null)
            {
                return new PaysheetDto();
            }

            // Lấy thời gian ngày công chuẩn trong thiết lập cửa hàng
            var settingsObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId), cancellationToken);
            var timeOfStandardWorkingDay = settingsObjectDto.StandardWorkingDay;
            var paySheetDto = _mapper.Map<PaysheetDto>(paySheet);
            paySheetDto.Code = paySheet.PaysheetStatus == (byte)PaysheetStatuses.Draft ? string.Empty : paySheet.Code;
            paySheetDto.PaysheetStatus = (byte)PaysheetStatuses.TemporarySalary;
            paySheetDto.TimeOfStandardWorkingDay = timeOfStandardWorkingDay;

            var payslipSpec = new GetPayslipByPaysheetId(request.PaySheetId)
                .And(new FindPayslipByIdSpec(request.PayslipId))
                .And(new FindPayslipByEmployeeId(request.EmployeeId));

            var payslips = await _payslipReadOnlyRepository.GetBySpecificationAsync(payslipSpec, true);

            var employee = await _employeeReadOnlyRepository.FindAndIncludeBySpecificationAsync(
                new FindEmployeeByIdSpec(request.EmployeeId),
                new string[] { nameof(EmployeeDto.Department), nameof(EmployeeDto.JobTitle) }, true);
            var employeeDto = employee != null ? _mapper.Map<EmployeeDto>(employee) : null;

            var payslipsDtoList = new List<PayslipDto>();
            payslips.ForEach(payslip =>
            {
                var payslipDto = _mapper.Map<PayslipDto>(payslip);
                payslipDto.Employee = employeeDto;
                payslipDto.Code = payslipDto.PayslipStatus == (byte)PayslipStatuses.Draft
                    ? string.Empty
                    : payslipDto.Code;
                payslipDto.PayslipStatus = (byte)PayslipStatuses.TemporarySalary;
                payslipDto.PayslipClockings = _mapper.Map<List<PayslipClockingDto>>(payslip.PayslipClockings);
                payslipsDtoList.Add(payslipDto);
            });
            paySheetDto.Payslips = payslipsDtoList;

            return paySheetDto;
        }
    }
}
