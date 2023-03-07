using System.Collections.Generic;
using KiotVietTimeSheet.SharedKernel.ConfigModels;
using Microsoft.Extensions.Configuration;

namespace KiotVietTimeSheet.Application.Configuration
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        private readonly IConfiguration _configuration;
        public byte NationalHolidayCreationDay => _configuration.GetSection("NationalHolidayCreationDay").Get<byte>();
        public byte NationalHolidayCreationMonth => _configuration.GetSection("NationalHolidayCreationMonth").Get<byte>();
        public List<string> ClassApiCanCallWhenInactiveTimeSheets => _configuration.GetSection("ClassApiCanCallWhenInactiveTimeSheets").Get<List<string>>();
        public int EmployeesPerBlock => _configuration.GetSection("EmployeesPerBlock").Get<int>() != 0 ? _configuration.GetSection("EmployeesPerBlock").Get<int>() : 5;
        public List<string> ClassApiCanCallWhenTimeSheetFeatureExpired => _configuration.GetSection("ClassApiCanCallWhenTimeSheetFeatureExpired").Get<List<string>>();
        public int MaxUploadFileSize => _configuration.GetSection("MaxUploadFileSize").Get<int>();
        public FtpConfiguration Ftp => _configuration.GetSection("Ftp").Get<FtpConfiguration>();
        public TimeSheetValidateConfiguration TimeSheetValidate
        {
            get
            {
                var config = _configuration.GetSection("TimeSheetValidate").Get<TimeSheetValidateConfiguration>() ?? new TimeSheetValidateConfiguration();
                if (config.AllowOrderMaxDay == 0) config.AllowOrderMaxDay = 30;
                if (config.AllowOrderMaxMonth == 0) config.AllowOrderMaxMonth = 1;

                return config;
            }
        }

        public AllowUseClockingGpsConfiguration AllowUseClockingGpsConfiguration
        {
            get
            {
                var config = _configuration.GetSection("AllowUseClockingGps").Get<AllowUseClockingGpsConfiguration>() ?? new AllowUseClockingGpsConfiguration();

                if (config.IncludeTenantIds == null) config.IncludeTenantIds = new List<int>();
                if (config.ExcludeTenantIds == null) config.ExcludeTenantIds = new List<int>();

                return config;
            }
        }

        public bool IsCallExportRequestToKiotApi => _configuration.GetValue<bool>("IsCallRequestExportToKiotApi");
        public ApplicationConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
