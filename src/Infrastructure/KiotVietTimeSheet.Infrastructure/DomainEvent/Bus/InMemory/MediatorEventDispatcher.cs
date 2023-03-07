using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Infrastructure.DomainEvent.Bus.InMemory
{
    public class MediatorEventDispatcher : IEventDispatcher
    {
        private readonly IMediator _mediator;
        public MediatorEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task FireEvent<T>(T @event) where T : Message
        {
            return _mediator.Publish(@event);
        }
    }
}
