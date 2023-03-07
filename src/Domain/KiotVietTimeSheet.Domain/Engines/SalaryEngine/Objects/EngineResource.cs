using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects
{
    public class EngineResource
    {
        public List<Clocking> UnPaidClockings { get; set; }
        public List<Clocking> PaidClockings { get; set; }
        public List<ClockingPenalize> ClockingPenalizes { get; set; }
        public Employee Employee { get; set; }
        public BranchSetting BranchSetting { get; set; }
        public List<Holiday> Holidays { get; set; }
        public List<IRule> Rules { get; set; }
        public int TimeOfStandardWorkingDay { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCounselorRevenue { get; set; }
        public decimal TotalGrossProfit { get; set; }
        public int StandardWorkingDayNumber { get; set; }
        public byte PaySlipStatus { get; set; }
        public List<Deduction> Deductions { get; set; }
        public List<Allowance> Allowances { get; set; }
        public List<Shift> Shifts { get; set; }
        public List<ProductRevenue> ProductRevenues { get; set; }
        public List<ProductRevenue> ProductCounselorRevenues { get; set; }
        public List<CommissionTableParam> Commissions { get; set; }
        public decimal Bonus { get; set; }
        public List<ProductRevenue> BranchProductRevenues { get; set; }
        public SettingsToObject SettingsToObject { get; set; }
        public int NumberLateTimeHaftWorkingDay { get; set; }
        public int NumberEarlyTimeHaftWorkingDay { get; set; }
        public List<DateTime> HalfShiftDays { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
