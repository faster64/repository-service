using KiotVietTimeSheet.Infrastructure.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class AddMediatorBehaviorsExtension
    {
        public static void AddMediatorBehaviors(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ProfilerBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        }
    }
}
