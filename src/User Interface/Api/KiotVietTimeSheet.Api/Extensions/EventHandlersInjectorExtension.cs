using KiotVietTimeSheet.Application.EventHandlers.DomainEventHandlers;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class EventHandlersInjectorExtension
    {
        public static void AddEventHandlers(this IServiceCollection services)
        {
            services.AddTransient<INotificationHandler<CancelPaysheetEvent>, PaysheetDomainEventHandlers>();

            services.AddTransient<INotificationHandler<SwappedClockingEvent>, ClockingDomainEventHandlers>();

            services.AddTransient<INotificationHandler<CreatedShiftEvent>, ShiftDomainEventHandlers>();
            services.AddTransient<INotificationHandler<UpdatedShiftEvent>, ShiftDomainEventHandlers>();
            services.AddTransient<INotificationHandler<DeletedShiftEvent>, ShiftDomainEventHandlers>();

            services.AddTransient<INotificationHandler<CancelPayslipEvent>, PayslipDomainEventHandlers>();
            services.AddTransient<INotificationHandler<CreatedPayRateEvent>, PayRateDomainEventHandlers>();
            services.AddTransient<INotificationHandler<UpdatedPayRateEvent>, PayRateDomainEventHandlers>();
            services.AddTransient<INotificationHandler<UpdateSettingsEvent>, SettingDomainEventHandlers>();

            services.AddTransient<INotificationHandler<CreatedHolidayEvent>, HolidayDomainEventHandlers>();
            services.AddTransient<INotificationHandler<UpdatedHolidayEvent>, HolidayDomainEventHandlers>();
            services.AddTransient<INotificationHandler<DeletedHolidayEvent>, HolidayDomainEventHandlers>();

            services.AddTransient<INotificationHandler<DeletedEmployeeEvent>, EmployeeDomainEventHandlers>();

            services.AddTransient<INotificationHandler<CreatedCommissionEvent>, CommissionDomainEventHandlers>();
            services.AddTransient<INotificationHandler<UpdatedCommissionEvent>, CommissionDomainEventHandlers>();
            services.AddTransient<INotificationHandler<DeletedCommissionEvent>, CommissionDomainEventHandlers>();

            services.AddTransient<INotificationHandler<CreatedPayRateTemplateEvent>, PayRateTemplateDomainEventHandlers>();
            services.AddTransient<INotificationHandler<UpdatedPayRateTemplateEvent>, PayRateTemplateDomainEventHandlers>();

        }
    }
}
