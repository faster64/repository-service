using System.Collections.Generic;
using System.Linq;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary
{
    public class MainSalaryRuleValueDetail
    {
        public long ShiftId { get; set; }
        public decimal Default { get; set; }
        public List<MainSalaryHolidays> MainSalaryHolidays { get; set; }
        public int Rank { get; set; }

        public bool IsEqual(MainSalaryRuleValueDetail detail) =>
            detail != null &&
            ShiftId == detail.ShiftId &&
            Default == detail.Default &&
            MainSalaryHolidays != null && detail.MainSalaryHolidays!=null && MainSalaryHolidays.Count> 0 && detail.MainSalaryHolidays.Count>0 &&
            MainSalaryHolidays.TrueForAll(holiday => detail.MainSalaryHolidays.Any(holiday.IsEqual));
    }
}