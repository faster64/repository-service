using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Application.EventHandlers.DomainEventHandlers
{
    public class SettingDomainEventHandlers : IEventHandler<UpdateSettingsEvent>
    {
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;
        private readonly IAuthService _authService;

        public SettingDomainEventHandlers(
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService,
            IAuthService authService)
        {
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
            _authService = authService;
        }

        public async Task Handle(UpdateSettingsEvent notification, CancellationToken cancellationToken)
        {
            if (notification.Settings.Name == nameof(SettingsToObject.StandardWorkingDay)
                && notification.Settings.Value != notification.OldSettings.Value)
                await _paySheetOutOfDateDomainService.WithSettingsChangeAsync(_authService.Context.BranchId);
        }
    }
}
