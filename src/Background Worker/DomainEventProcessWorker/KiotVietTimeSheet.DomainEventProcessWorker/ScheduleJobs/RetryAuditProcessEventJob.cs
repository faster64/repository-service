using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Funq;
using KiotVietTimeSheet.Application.EventBus.Events.ShiftEvents;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Quartz;
using ServiceStack;

namespace KiotVietTimeSheet.DomainEventProcessWorker.ScheduleJobs
{
    [DisallowConcurrentExecution]
    public class RetryAuditProcessEventJob : IJob
    {

        private ILogger<RetryAuditProcessEventJob> _logger;
        private ILogger<RetryAuditProcessEventJob> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<RetryAuditProcessEventJob>>());

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var auditProcessFail = HostContext.Resolve<IAuditProcessFailEventService>();
                try
                {
                    await auditProcessFail.RetryAuditProcessFailEvents(async e =>
                    {
                        var eventShiftType = Assembly.GetAssembly(typeof(CreatedShiftIntegrationEvent))
                            .GetTypes()
                            .FirstOrDefault(f => f.Name == e.GetType().Name);
                        if (eventShiftType == null) return;
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventShiftType);
                        var handlerShift = HostContext.Container.TryResolve(concreteType);
                        if (handlerShift == null) return;
                        var method = concreteType.GetMethod("Handle")?.Invoke(handlerShift, new object[] { e });
                        if (method == null) return;
                        await (Task)method;
                    });
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }
    }

    public static class ContainerExtensions
    {
        public static object TryResolve(this Container container, Type type)
        {
            var mi = typeof(Container).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .First(x => x.Name == "TryResolve" &&
                            x.GetGenericArguments().Length == 1 &&
                            x.GetParameters().Length == 0);

            var genericMi = mi.MakeGenericMethod(type);
            var instance = genericMi.Invoke(container, new object[0]);
            return instance;
        }
    }
}
