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

namespace KiotVietTimeSheet.Application.Commands.UpdatePaySheetPayslips
{
    public class UpdatePaySheetPayslipsCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdatePaySheetPayslipsCommand, PaysheetDto>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdatePaySheetPayslipsCommandHandler> _logger;
        private readonly IBranchSettingReadOnlyRepository _branchSettingReadOnlyRepository;
        private readonly IPaysheetWriteOnlyRepository _paySheetWriteOnlyRepository;
        private readonly IPayslipWriteOnlyRepository _payslipWriteOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paySheetReadOnlyRepository;
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;

        public UpdatePaySheetPayslipsCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IBranchSettingReadOnlyRepository branchSettingReadOnlyRepository,
            IPaysheetWriteOnlyRepository paySheetWriteOnlyRepository,
            IPaysheetReadOnlyRepository paySheetReadOnlyRepository,
            IPayslipWriteOnlyRepository payslipWriteOnlyRepository,
            ILogger<UpdatePaySheetPayslipsCommandHandler> logger,
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService
        ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _logger = logger;
            _paySheetWriteOnlyRepository = paySheetWriteOnlyRepository;
            _branchSettingReadOnlyRepository = branchSettingReadOnlyRepository;
            _paySheetReadOnlyRepository = paySheetReadOnlyRepository;
            _payslipWriteOnlyRepository = payslipWriteOnlyRepository;
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
        }

        public async Task<PaysheetDto> Handle(UpdatePaySheetPayslipsCommand request, CancellationToken cancellationToken)
        {
            // Lấy settings của chi nhánh
            var paySheet = await _paySheetReadOnlyRepository.GetPaysheetWithoutVoidPayslipById(request.PaySheetDto.Id);
            try
            {
                var branchSetting = await _branchSettingReadOnlyRepository.FindBranchSettingWithDefault(new FindBranchSettingByBranchIdSpec(request.PaySheetDto.BranchId));

                var draftPayslips = new List<DraftPayslipDomainServiceDto>();
                var employees = _mapper.Map<List<Employee>>(request.EmployeesDto);
                var commissionTables = _mapper.Map<List<Commission>>(request.CommissionsDto);
                var listUnPaidClockings = _mapper.Map<List<Clocking>>(request.ClockingsUnPaid);
                var listPaidClockings = _mapper.Map<List<Clocking>>(request.ClockingsPaid);
                var listAllowances = _mapper.Map<List<Allowance>>(request.AllowancesDto);
                var listDeductions = _mapper.Map<List<Deduction>>(request.DeductionsDto);
                var listHolidays = _mapper.Map<List<Holiday>>(request.HolidaysDto);
                var listShifts = _mapper.Map<List<Shift>>(request.ShiftsDto);
                var clockingPenalizesDto = _mapper.Map<List<ClockingPenalize>>(request.ClockingPenalizesDto);
                var settingToObject = _mapper.Map<SettingsToObject>(request.SettingObjectDto);

                listShifts = listShifts.Where(x => !x.IsDeleted).ToList();

                var payslips = paySheet.Payslips;
                var paysheetCreatedDate = DateTime.Now;
                employees.ForEach(employee =>
                {
                    var mapperCommissionTables = commissionTables.Select(x => new CommissionTableParam(x)).ToList();
                    employee.Clockings = listUnPaidClockings.Where(x => x.EmployeeId == employee.Id).ToList();
                    var payRate = request.PayRates.FirstOrDefault(p => p.EmployeeId == employee.Id && p.SalaryPeriod == request.PaySheetDto.SalaryPeriod);
                    if (!_paySheetOutOfDateDomainService.IsCheckPayRateDetail(payRate, listAllowances, listDeductions)) return;
                    var payslipClockingPenalizes = clockingPenalizesDto.Where(x => x.EmployeeId == employee.Id).ToList();

                    var rules = payRate?.PayRateDetails.Select(prd => RuleFactory.GetRule(prd.RuleType, prd.RuleValue)).ToList();
                    rules = RuleFactory.OrderRules(rules);


                    var engineResource = new EngineResource
                    {
                        Employee = employee,
                        ClockingPenalizes = payslipClockingPenalizes,
                        BranchSetting = branchSetting,
                        UnPaidClockings = listUnPaidClockings.Where(c => c.EmployeeId == employee.Id).ToList(),
                        PaidClockings = listPaidClockings.Where(c => c.EmployeeId == employee.Id).ToList(),
                        Holidays = listHolidays,
                        TimeOfStandardWorkingDay = request.PaySheetDto.TimeOfStandardWorkingDay,
                        StandardWorkingDayNumber = request.PaySheetDto.WorkingDayNumber,
                        //TotalRevenue = request.UserRevenues.FirstOrDefault(u => u.EmployeeId == employee.Id)?.NetSale ?? 0,
                        TotalRevenue = request.UserProductRevenues.Where(x => x.EmployeeId == employee.Id).Sum(u => u.TotalCommission),
                        TotalCounselorRevenue = request.UserCounselorRevenues.FirstOrDefault(u => u.EmployeeId == employee.Id)?.NetSale ?? 0,
                        TotalGrossProfit = request.UserRevenues.FirstOrDefault(u => u.EmployeeId == employee.Id)?.TotalGrossProfit ?? 0,
                        Rules = rules,
                        PaySlipStatus = (byte)PayslipStatuses.TemporarySalary,
                        Deductions = listDeductions,
                        Allowances = listAllowances,
                        Shifts = listShifts,
                        Commissions = mapperCommissionTables,
                        ProductRevenues = request.UserProductRevenues.Where(x => x.EmployeeId == employee.Id).ToList(),
                        ProductCounselorRevenues = request.UserCounselorProductRevenues.Where(x => x.EmployeeId == employee.Id).ToList(),
                        BranchProductRevenues = request.BranchProductRevenues,
                        SettingsToObject = settingToObject,
                        HalfShiftDays = new List<DateTime>(),
                        StartTime = request.PaySheetDto.StartTime,
                        EndTime = request.PaySheetDto.EndTime,
                    };

                    var payslip = GetPayslip(engineResource, employee, rules, request.PaySheetDto.CreatorBy, paysheetCreatedDate, payslips);

                    if (payslip == null) return;

                    payslip.PayslipClockings = (from c in request.ClockingsDto
                                                where c.EmployeeId == payslip.EmployeeId
                                                select new PayslipClocking(
                                                    payslip.Id,
                                                    c.Id,
                                                    c.CheckInDate,
                                                    c.CheckOutDate,
                                                    (!engineResource.HalfShiftDays.Contains(c.StartTime.Date) ? c.TimeIsLate : 0),
                                                    c.OverTimeBeforeShiftWork,
                                                    (!engineResource.HalfShiftDays.Contains(c.StartTime.Date) ? c.TimeIsLeaveWorkEarly : 0),
                                                    c.OverTimeAfterShiftWork,
                                                    c.AbsenceType,
                                                    c.ClockingStatus,
                                                    c.StartTime,
                                                    c.EndTime,
                                                    c.ShiftId
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

                    payslip.PayslipPenalizes =
                        (from cp in payslipClockingPenalizes
                            group cp by new {cp.PenalizeId, cp.MoneyType}
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

                var listPaySlipIsCreated = draftPayslips.Select(x => x.Payslip).ToList();
                paySheet.PaysheetStatus = (byte)PaysheetStatuses.TemporarySalary;
                paySheet.PaysheetCreatedDate = paysheetCreatedDate;
                paySheet.TimeOfStandardWorkingDay = request.PaySheetDto.TimeOfStandardWorkingDay;
                paySheet.WorkingDayNumber = request.PaySheetDto.WorkingDayNumber;
                paySheet.ErrorStatus = null;
                if (listPaySlipIsCreated.Any())
                {
                    paySheet.Payslips = listPaySlipIsCreated;
                    BatchDeletePayslips(listPaySlipIsCreated, payslips);
                    await _paySheetWriteOnlyRepository.StoreAsync(paySheet);
                }
                else
                {
                    _paySheetWriteOnlyRepository.Update(paySheet);
                }

                await _paySheetWriteOnlyRepository.UnitOfWork.CommitBySaveChangesAsync();
                return _mapper.Map<PaysheetDto>(paySheet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private void BatchDeletePayslips(List<Payslip> listPaySlipIsCreated, List<Payslip> payslipFromSources)
        {
            var listPayslipTemp = listPaySlipIsCreated.Select(x => x.Id).ToList();
            var payslipDeletes = payslipFromSources.Where(x => !listPayslipTemp.Contains(x.Id)).ToList();
            if (!payslipDeletes.Any())
            {
                return;
            }

            _payslipWriteOnlyRepository.BatchDelete(payslipDeletes);
        }

        private Payslip GetPayslip(EngineResource engineResource, Employee employee, List<IRule> rules, long? payslipCreatedBy, DateTime? payslipCreatedDate, List<Payslip> payslips)
        {
            engineResource.PaySlipStatus = (byte)PayslipStatuses.TemporarySalary;
            var payslip = payslips.FirstOrDefault(p => p.EmployeeId == employee.Id);

            if (payslip == null)
            {
                return new Payslip(rules, employee.Id, (byte)PayslipStatuses.TemporarySalary, engineResource, payslipCreatedBy, payslipCreatedDate, false);
            }

            engineResource.Bonus = payslip.Bonus;
            payslip.UpdateWhenDataChanged(rules, engineResource);
            payslip.PayslipClockings = null;

            return payslip;
        }
    }
}
