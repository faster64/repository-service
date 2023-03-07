using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary
{
    public class OvertimeSalaryByShiftParam
    {
        public long ShiftId { get; set; }
        public double ShiftHours { get; set; }
        public List<OvertimeSalaryByShiftParamDays> OvertimeSalaryByShiftParamDays { get; set; }
    }
}
