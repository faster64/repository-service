using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategoryAsync
{
    public class CreateCommissionDetailByProductCategoryAsyncCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateCommissionDetailByProductCategoryAsyncCommand, Unit>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public CreateCommissionDetailByProductCategoryAsyncCommandHandler(
            IEventDispatcher eventDispatcher,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<Unit> Handle(CreateCommissionDetailByProductCategoryAsyncCommand request, CancellationToken cancellationToken)
        {
            var commissionIds = request.CommissionIds;
            var productCategory = request.ProductCategory;
            if (!commissionIds.Any())
            {
                NotifyCommissionHaveNotSelected();
            }
            await _timeSheetIntegrationEventService.AddEventAsync(
                new CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent(commissionIds,
                    productCategory));
            return Unit.Value;
        }
        private void NotifyCommissionHaveNotSelected()
        {
            _eventDispatcher.FireEvent(new DomainNotification(nameof(CommissionDetail), Message.commission_haveNotSelected));
        }
    }
}