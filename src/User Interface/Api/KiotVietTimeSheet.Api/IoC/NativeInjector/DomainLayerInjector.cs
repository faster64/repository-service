using KiotVietTimeSheet.Infrastructure.DomainEvent.Bus.InMemory;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.IoC.NativeInjector
{
    public class DomainLayerInjector
    {
        protected DomainLayerInjector(){}
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            // Mediator dispatcher
            services.AddScoped<IEventDispatcher, MediatorEventDispatcher>();

            // Domain Notifications
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
        }
    }
}
