using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetSetting
{
    public class GetSettingQueryHandler : QueryHandlerBase,
        IRequestHandler<GetSettingQuery, SettingObjectDto>
    {
        private readonly ISettingsReadOnlyRepository _settingsRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public GetSettingQueryHandler(
            IAuthService authService,
            ISettingsReadOnlyRepository settingsRepository,
            IApplicationConfiguration applicationConfiguration) : base(authService)
        {
            _settingsRepository = settingsRepository;
            _applicationConfiguration = applicationConfiguration;
        }

        public async Task<SettingObjectDto> Handle(GetSettingQuery request, CancellationToken cancellationToken)
        {
            SettingObjectDto settingData = new SettingObjectDto();
            var setting =  await _settingsRepository.GetBySpecificationAsync(new FindSettingByTenantIdSpec(request.TenantId));
            settingData.Data = setting.ToDictionary(x => x.Name, x => x.Value);

            // Set allow use Clocking Gps
            settingData.IsAllowUseClockingGps = GetIsAllowUseClockingGps(request.TenantId);

            return settingData;
        }

        private bool GetIsAllowUseClockingGps(int tenantId)
        {
            if (!_applicationConfiguration.AllowUseClockingGpsConfiguration.IsEnableFilter) return true;

            var result = _applicationConfiguration.AllowUseClockingGpsConfiguration.ExcludeTenantIds.All(x => x != tenantId)
                         && _applicationConfiguration.AllowUseClockingGpsConfiguration.IncludeTenantIds.Any(x => x == tenantId);

            return result;
        }
    }
}
