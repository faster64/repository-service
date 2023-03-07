using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.BackgroundTasks.EventHandlers
{
    public class CommissionDetailIntegrationEventHandler :
        IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent>
    {
        private readonly CommissionDetailProcess _commissionDetailProcess;
        

        public CommissionDetailIntegrationEventHandler(CommissionDetailProcess commissionDetailProcess)
        {
            _commissionDetailProcess = commissionDetailProcess;
        }

        public async Task Handle(CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent @event)
        {
            await _commissionDetailProcess.CreateCommissionDetailByProductCategoryAsync(@event);
        }

    }
}
