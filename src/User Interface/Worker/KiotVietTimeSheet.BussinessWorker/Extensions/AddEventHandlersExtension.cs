using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.BussinessWorker.EventHandlers;
using KiotVietTimeSheet.BussinessWorker.IntegrationEvents.Events;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.BussinessWorker.Extensions
{
    public static class AddEventHandlersExtension
    {
        public static void AddEventHandlers(this IServiceCollection services)
        {
            services.AddScoped<IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent>, CommissionDetailIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<CreatedPayslipPaymentIntegrationEvent>, PayslipPaymentIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<VoidedPayslipPaymentIntegrationEvent>, PayslipPaymentIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<CheckUpdateTenantNationalHolidayIntegrationEvent>, HolidayIntegrationEventHandler>();

            services.AddTransient<IIntegrationEventHandler<CreatePaysheetEmptyIntegrationEvent>, PaySheetIntegrationEventHandler>();

            //registry Active Timesheet
            services.AddTransient<IIntegrationEventHandler<ActivedFeatureIntegrationEvent>, ActivedFeatureIntegrationEventHandler>();
        }
    }
}
