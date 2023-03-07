using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.BackgroundTasks.EventHandlers
{
    public class PaySheetIntegrationEventHandler : 
        IIntegrationEventHandler<CreatePaysheetEmptyIntegrationEvent>,
        IIntegrationEventHandler<AutoLoadingAndUpdatePaysheetIntegrationEvent>,
        IIntegrationEventHandler<ChangePeriodPaysheetIntegrationEvent>
    {
        private readonly ILogger<CreatePaysheetEmptyIntegrationEvent> _logger;
        private readonly PaySheetProcess _paySheetProcess;

        public PaySheetIntegrationEventHandler(
            ILogger<CreatePaysheetEmptyIntegrationEvent> logger,
            PaySheetProcess paySheetProcess)
        {
            _logger = logger;
            _paySheetProcess = paySheetProcess;
        }

        public async Task Handle(CreatePaysheetEmptyIntegrationEvent @event)
        {
            try
            {
                await _paySheetProcess.CreatePaySheet(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task Handle(AutoLoadingAndUpdatePaysheetIntegrationEvent @event)
        {
            try
            {
                await _paySheetProcess.AutoLoadingAndUpdatePaySheet(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task Handle(ChangePeriodPaysheetIntegrationEvent @event)
        {
            try
            {
                await _paySheetProcess.ChangePeriodPaySheet(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

        }
    }
}
