using System.Collections.Generic;
using KiotVietTimeSheet.SharedKernel.ConfigModels;

namespace KiotVietTimeSheet.Application.Configuration
{
    public interface IApplicationConfiguration
    {
        byte NationalHolidayCreationDay { get; }
        byte NationalHolidayCreationMonth { get; }
        List<string> ClassApiCanCallWhenInactiveTimeSheets { get; }
        int EmployeesPerBlock { get; }
        List<string> ClassApiCanCallWhenTimeSheetFeatureExpired { get; }
        TimeSheetValidateConfiguration TimeSheetValidate { get; }
        int MaxUploadFileSize { get; }
        FtpConfiguration Ftp { get; }
        AllowUseClockingGpsConfiguration AllowUseClockingGpsConfiguration { get; }
        bool IsCallExportRequestToKiotApi { get; }
    }
}
