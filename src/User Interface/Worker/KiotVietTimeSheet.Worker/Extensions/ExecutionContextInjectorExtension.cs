using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.SharedKernel.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.AuditTrailWorker.Extensions
{
    public static class ExecutionContextInjectorExtension
    {
        public static void AddExecutionContext(this IServiceCollection services)
        {
            services.AddScoped(c =>
            {
                var eventContextSvc = c.GetRequiredService<IEventContextService>();
                var context = new ExecutionContext
                {
                    TenantId = eventContextSvc.Context.TenantId,
                    TenantCode = eventContextSvc.Context.RetailerCode,
                    BranchId = eventContextSvc.Context.BranchId,
                    User = eventContextSvc.Context.User,
                    Language = eventContextSvc.Context.Language ?? "vi-VN"
                };

                return context;
            });
        }
    }
}
