using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary
{
    public class MainSalaryByShiftParam
    {
        public long ShiftId { get; set; }
        public decimal Salary { get; set; }
        public decimal CalculatedSalary { get; set; }
        public decimal Default { get; set; }
        public decimal CalculatedDefault { get; set; }
        public List<MainSalaryByShiftParamDetail> MainSalaryByShiftParamDetails { get; set; }
        public MainSalaryTypes Type { get; set; }
    }
}