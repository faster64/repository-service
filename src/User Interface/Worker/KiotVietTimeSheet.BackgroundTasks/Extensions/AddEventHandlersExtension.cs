using KiotVietTimeSheet.Application.EventBus.Events.AutoKeepingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.EventBus.Events.ConfirmClockingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.BackgroundTasks.EventHandlers;
using KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.BackgroundTasks.Extensions
{
    public static class AddEventHandlersExtension
    {
        public static void AddEventHandlers(this IServiceCollection services)
        {
            services.AddScoped<IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent>, CommissionDetailIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<CreatedPayslipPaymentIntegrationEvent>, PayslipPaymentIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<VoidedPayslipPaymentIntegrationEvent>, PayslipPaymentIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<CheckUpdateTenantNationalHolidayIntegrationEvent>, HolidayIntegrationEventHandler>();

            //registry Active Timesheet
            services.AddTransient<IIntegrationEventHandler<ActivedFeatureIntegrationEvent>, ActivedFeatureIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<SentMailIntegrationEvent>, SentMailIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<CreatePaysheetEmptyIntegrationEvent>, PaySheetIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<AutoLoadingAndUpdatePaysheetIntegrationEvent>, PaySheetIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<ChangePeriodPaysheetIntegrationEvent>, PaySheetIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<CreateAutoGenerateClockingIntegrationEvent>, AutoGenerateClockingIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<UpdateClockingTimeIntegrationEvent>, UpdateClockingTimeIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<AutoKeepingIntegrationEvent>, AutoKeepingIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<CreatedConfirmClockingIntegrationEvent>, ConfirmClockingIntegrationEventHandler>();
        }
    }
}
