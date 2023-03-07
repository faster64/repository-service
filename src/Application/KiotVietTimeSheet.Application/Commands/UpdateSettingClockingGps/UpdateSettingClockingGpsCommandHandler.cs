using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.EventBus.Events.SettingEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdateSettingClockingGps
{
    public class UpdateSettingClockingGpsCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateSettingClockingGpsCommand, bool>
    {
        private readonly IAuthService _authService;
        private readonly ISettingsWriteOnlyRepository _settingsWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        public UpdateSettingClockingGpsCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            ISettingsWriteOnlyRepository settingsWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        )
            : base(eventDispatcher)
        {
            _authService = authService;
            _settingsWriteOnlyRepository = settingsWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<bool> Handle(UpdateSettingClockingGpsCommand request, CancellationToken cancellationToken)
        {
            var name = nameof(SettingsToObject.UseClockingGps);
            var setting = await _settingsWriteOnlyRepository.FindBySpecificationAsync(new FindSettingByNameSpec(name));
            if (setting == null)
            {
                var newSetting = Settings.CreateInstance(_authService.Context.TenantId, name, request.UseClockingGps.ToString());
                _settingsWriteOnlyRepository.Add(newSetting);
            }
            else if (setting.Value != request.UseClockingGps.ToString())
            {
                setting.Update(request.UseClockingGps.ToString());
                _settingsWriteOnlyRepository.Update(setting);
            }
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdateSettingClockingGpsIntegrationEvent(request.UseClockingGps));

            await _settingsWriteOnlyRepository.UnitOfWork.CommitAsync();

            return true;
        }

    }
}
