using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    /// <summary>
    /// Find all time sheet by id of shift
    /// </summary>
    public class FindTimeSheetByShiftIdSpec : ExpressionSpecification<TimeSheet>
    {
        #region Constructors

        public FindTimeSheetByShiftIdSpec(long shiftId)
            : base(c => c.TimeSheetShifts.SelectMany(x => x.ShiftIdsToList).Contains(shiftId))
        {
        }

        #endregion
    }
}
