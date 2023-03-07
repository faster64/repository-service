using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Threading;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Specifications;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using System;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using MediatR;
using KiotVietTimeSheet.SharedKernel.Domain;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.CreatePaysheetPayslip
{
    public class CreatePaySheetPayslipCommandHandler : BaseCommandHandler,
        IRequestHandler<CreatePaySheetPayslipCommand, PaysheetDto>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CreatePaySheetPayslipCommandHandler> _logger;
        private readonly IBranchSettingReadOnlyRepository _branchSettingReadOnlyRepository;
        private readonly IPaysheetWriteOnlyRepository _paySheetWriteOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paySheetReadOnlyRepository;
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;

        public CreatePaySheetPayslipCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IBranchSettingReadOnlyRepository branchSettingReadOnlyRepository,
            IPaysheetWriteOnlyRepository paySheetWriteOnlyRepository,
            IPaysheetReadOnlyRepository paySheetReadOnlyRepository,
            ILogger<CreatePaySheetPayslipCommandHandler> logger,
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService
        ) :base (eventDispatcher)
        {
            _mapper = mapper;
            _logger = logger;
            _paySheetWriteOnlyRepository = paySheetWriteOnlyRepository;
            _branchSettingReadOnlyRepository = branchSettingReadOnlyRepository;
            _paySheetReadOnlyRepository = paySheetReadOnlyRepository;
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
        }

        public async Task<PaysheetDto> Handle(CreatePaySheetPayslipCommand request, CancellationToken cancellationToken)
        {
            // Lấy settings của chi nhánh
            var paySheet = await _paySheetReadOnlyRepository.FindByIdAsync(request.PaySheetDto.Id);
            try
            {
                var branchSetting = await _branchSettingReadOnlyRepository.FindBranchSettingWithDefault(new FindBranchSettingByBranchIdSpec(request.PaySheetDto.BranchId));

                var draftPayslips = new List<DraftPayslipDomainServiceDto>();
                var employees = _mapper.Map<List<Employee>>(request.ListEmployeeDto);
                var commissionTables = _mapper.Map<List<Commission>>(request.ListCommissionDto);
                var unPaidClockings = _mapper.Map<List<Clocking>>(request.ListClockingUnPaid);
                var paidClockings = _mapper.Map<List<Clocking>>(request.ListClockingPaid);
                var allowances = _mapper.Map<List<Allowance>>(request.ListAllowanceDto);
                var deductions = _mapper.Map<List<Deduction>>(request.ListDeductionDto);
                var holidays = _mapper.Map<List<Holiday>>(request.ListHolidayDto);
                var shifts = _mapper.Map<List<Shift>>(request.ListShiftDto);
                var clockingPenalizesDto = _mapper.Map<List<ClockingPenalize>>(request.ClockingPenalizesDto);
                var settingToObject = _mapper.Map<SettingsToObject>(request.SettingObjectDto);

                shifts = shifts.Where(x => !x.IsDeleted).ToList();

                var paysheetCreatedDate = DateTime.Now;

                employees.ForEach(employee =>
                {
                    var mapperCommissionTables = commissionTables.Select(x => new CommissionTableParam(x)).ToList();
                    employee.Clockings = unPaidClockings.Where(x => x.EmployeeId == employee.Id).ToList();
                    var payRate = request.PayRates.FirstOrDefault(p => p.EmployeeId == employee.Id && p.SalaryPeriod == request.PaySheetDto.SalaryPeriod);
                    if (!_paySheetOutOfDateDomainService.IsCheckPayRateDetail(payRate, allowances, deductions)) return;
                    var payslipClockingPenalizes = clockingPenalizesDto.Where(x => x.EmployeeId == employee.Id).ToList();

                    var rules = payRate?.PayRateDetails.Select(prd => RuleFactory.GetRule(prd.RuleType, prd.RuleValue)).ToList();
                    rules = RuleFactory.OrderRules(rules);

                    var engineResourceCreate = new EngineResource
                    {
                        Employee = employee,
                        ClockingPenalizes = payslipClockingPenalizes,
                        BranchSetting = branchSetting,
                        UnPaidClockings = unPaidClockings.Where(c => c.EmployeeId == employee.Id).ToList(),
                        PaidClockings = paidClockings.Where(c => c.EmployeeId == employee.Id).ToList(),
                        Holidays = holidays,
                        TimeOfStandardWorkingDay = request.PaySheetDto.TimeOfStandardWorkingDay,
                        StandardWorkingDayNumber = request.PaySheetDto.WorkingDayNumber,
                        //TotalRevenue = request.UserRevenues.FirstOrDefault(u => u.EmployeeId == employee.Id)?.NetSale ?? 0,
                        TotalRevenue = request.UserProductRevenues.Where(x => x.EmployeeId == employee.Id).Sum(u => u.TotalCommission),
                        TotalCounselorRevenue = request.UserCounselorRevenues.FirstOrDefault(u => u.EmployeeId == employee.Id)?.NetSale ?? 0,
                        TotalGrossProfit = request.UserRevenues.FirstOrDefault(u => u.EmployeeId == employee.Id)?.TotalGrossProfit ?? 0,
                        Rules = rules,
                        PaySlipStatus = (byte)PayslipStatuses.TemporarySalary,
                        Deductions = deductions,
                        Allowances = allowances,
                        Shifts = shifts,
                        Commissions = mapperCommissionTables,
                        ProductRevenues = request.UserProductRevenues.Where(x => x.EmployeeId == employee.Id).ToList(),
                        ProductCounselorRevenues = request.UserCounselorProductRevenues.Where(x => x.EmployeeId == employee.Id).ToList(),
                        BranchProductRevenues = request.BranchProductRevenues,
                        SettingsToObject = settingToObject,
                        HalfShiftDays = new List<DateTime>(),
                        StartTime = request.PaySheetDto.StartTime,
                        EndTime = request.PaySheetDto.EndTime,
                    };

                    var payslip = GetPayslip(engineResourceCreate, employee, rules, request.PaySheetDto.CreatorBy, paysheetCreatedDate);

                    if (payslip == null) return;

                    payslip.PayslipClockings = (from clocking in unPaidClockings
                                                where clocking.EmployeeId == payslip.EmployeeId
                                                select new PayslipClocking(
                                                    payslip.Id,
                                                    clocking.Id,
                                                    clocking.CheckInDate,
                                                    clocking.CheckOutDate,
                                                    // bỏ qua đi trẽ về sớm với những ngày làm nữa công
                                                    (!engineResourceCreate.HalfShiftDays.Contains(clocking.StartTime.Date) ? clocking.TimeIsLate : 0),
                                                    clocking.OverTimeBeforeShiftWork,
                                                    // bỏ qua đi trẽ về sớm với những ngày làm nữa công
                                                    (!engineResourceCreate.HalfShiftDays.Contains(clocking.StartTime.Date) ? clocking.TimeIsLeaveWorkEarly : 0),
                                                    clocking.OverTimeAfterShiftWork,
                                                    clocking.AbsenceType,
                                                    clocking.ClockingStatus,
                                                    clocking.StartTime,
                                                    clocking.EndTime,
                                                    clocking.ShiftId
                                                )).ToList();

                    payslip.PayslipPenalizes = (from cp in payslipClockingPenalizes
                            group cp by new {cp.PenalizeId, cp.MoneyType}
                            into grCp
                            select new PayslipPenalize(
                                payslip.Id,
                                grCp.Key.PenalizeId,
                                grCp.Sum(x => x.TimesCount * x.Value),
                                grCp.Sum(x => x.TimesCount),
                                grCp.Key.MoneyType,
                                true
                                )).ToList();

                    payslip.PayslipClockingPenalizes =
                        (from cp in request.ClockingPenalizesDto.Where(x => x.EmployeeId == employee.Id).ToList()
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

                    draftPayslips.Add(new DraftPayslipDomainServiceDto
                    {
                        Payslip = payslip,
                        Employee = employee,
                    });
                });

                var paySlipIsCreated = draftPayslips.Select(x => x.Payslip).ToList();
                paySheet.PaysheetStatus = (byte)PaysheetStatuses.TemporarySalary;
                paySheet.PaysheetCreatedDate = paysheetCreatedDate;
                if (paySlipIsCreated.Any())
                {
                    paySheet.Payslips = paySlipIsCreated;
                    await _paySheetWriteOnlyRepository.StoreAsync(paySheet);
                }
                else
                {
                    _paySheetWriteOnlyRepository.Update(paySheet);
                }

                await _paySheetWriteOnlyRepository.UnitOfWork.CommitAsync();
                return _mapper.Map<PaysheetDto>(paySheet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                paySheet.PaysheetStatus = (byte)PaysheetStatuses.Void;
                _paySheetWriteOnlyRepository.Update(paySheet);
                await _paySheetWriteOnlyRepository.UnitOfWork.CommitAsync();
                throw;
            }
        }

        private Payslip GetPayslip(EngineResource engineResource, Employee employee, List<IRule> rules, long? payslipCreatedBy, DateTime? payslipCreatedDate)
        {
            var payslip = new Payslip(rules, employee.Id, (byte)PayslipStatuses.TemporarySalary, engineResource, payslipCreatedBy, payslipCreatedDate, false);
            
            return payslip;
        }
    }
}
