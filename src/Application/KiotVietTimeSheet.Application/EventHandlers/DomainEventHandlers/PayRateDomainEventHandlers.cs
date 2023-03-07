using System.Collections.Generic;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.EventHandlers.DomainEventHandlers
{
    public class PayRateDomainEventHandlers : IEventHandler<CreatedPayRateEvent>, IEventHandler<UpdatedPayRateEvent>
    {
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;

        public PayRateDomainEventHandlers(
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService)
        {
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
        }

        public async Task Handle(CreatedPayRateEvent notification, CancellationToken cancellationToken)
        {
            await _paySheetOutOfDateDomainService.WithPayRateDataChangeAsync(new List<long> { notification.PayRate.EmployeeId });
        }

        public async Task Handle(UpdatedPayRateEvent notification, CancellationToken cancellationToken)
        {
            if (!notification.OldPayRate.IsEqual(notification.NewPayRate))
                await _paySheetOutOfDateDomainService.WithPayRateDataChangeAsync(new List<long> { notification.OldPayRate.EmployeeId });
        }
    }
}
