using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Validators;
using KiotVietTimeSheet.SharedKernel.ConfigModels;

namespace KiotVietTimeSheet.Application.Validators.TimeSheetValidators
{
    public class BatchUpdateTimeSheetsWhenUpdateDaysOffValidator : BaseTimeSheetValidator<List<TimeSheet>>
    {
        #region Properties
        #endregion

        public BatchUpdateTimeSheetsWhenUpdateDaysOffValidator(TimeSheetValidateConfiguration timeSheetValidateConfiguration)
        {
            ValidateStartDate();
            ValidateEndDate(timeSheetValidateConfiguration);
            ValidateRepeatDaysOfWeek();
            ValidateEmployeeId();
        }

        #region Protected methods

        #endregion
    }
}
