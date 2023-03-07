using AutoMapper;
using KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations
{
    public class AutoMapping
    {
        protected AutoMapping(){}
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingShiftToShiftDto());
                cfg.AddProfile(new MappingBranchSettingProfile());
                cfg.AddProfile(new MappingEmployeeToEmployeeDto());
                cfg.AddProfile(new MappingTimeSheetToTimeSheetDto());
                cfg.AddProfile(new PayRateTemplateToDtoProfile());
                cfg.AddProfile(new MappingAllowanceToDto());
                cfg.AddProfile(new MappingDeductionToDto());
                cfg.AddProfile(new MappingPayslipToPayslipDto());
                cfg.AddProfile(new PayRateToDtoProfile());
                cfg.AddProfile(new MappingHolidayToDto());
                cfg.AddProfile(new MappingFingerMachineToDto());
                cfg.AddProfile(new MappingFingerPrintToDto());
                cfg.AddProfile(new MappingCommissionDetailToCommissionDetailDto());
                cfg.AddProfile(new MappingCommissionBranchesToDto());
                cfg.AddProfile(new MappingCommissionToCommissionDto());
                cfg.AddProfile(new MappingClockingPenalizeToDto());
                cfg.AddProfile(new MappingPenalizeToDto());
                cfg.AddProfile(new MappingPayslipClockingPenalizeToDto());
                cfg.AddProfile(new MappingClockingToClockingDto());
                cfg.AddProfile(new MappingPayslipPenalizeToDto());
                cfg.AddProfile(new MappingConfirmClockingToDto());
                cfg.AddProfile(new MappingConfirmClockingHistoryToDto());
                cfg.AddProfile(new MappingGpsToDto());
                cfg.AddProfile(new MappingEmployeeToSyncEmployeeDto());
            });
        }
    }
}
