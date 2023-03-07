namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class PocoMapping
    {
        protected PocoMapping(){}
        public static void Factory()
        {
            HolidayMapping.Mapping();
            ShiftMapping.Mapping();
            EmployeeMapping.Mapping();
            EmployeeProfilePictureMapping.Mapping();
            TimeSheetMapping.Mapping();
            TimeSheetShiftMapping.Mapping();
            ClockingMapping.Mapping();
            DepartmentMapping.Mapping();
            JobTitleMapping.Mapping();
            ClockingHistoryMapping.Mapping();
            SettingsMapping.Mapping();
            BranchSettingMapping.Mapping();
            AllowanceMapping.Mapping();
            DeductionMapping.Mapping();
            PayRateMapping.Mapping();
            PaysheetMapping.Mapping();
            PayslipMapping.Mapping();
            PayRateTemplateMapping.Mapping();
            FingerPrintMapping.Mapping();
            CommissionMapping.Mapping();
            CommissionBranchMapping.Mapping();
            EmployeeBranchMapping.Mapping();
            ClockingPenalizeMapping.Mapping();
            PenalizeMapping.Mapping();
            GpsInfoMapping.Mapping();
            ConfirmClockingMapping.Mapping();
            ConfirmClockingHistoryMapping.Mapping();
        }
    }
}
