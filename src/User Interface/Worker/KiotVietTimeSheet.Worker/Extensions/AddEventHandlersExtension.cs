using KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.AuditTrailWorker.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using KiotVietTimeSheet.Application.EventBus.Events.AutoTimeKeepingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionEvents;
using KiotVietTimeSheet.Application.EventBus.Events.HolidayEvents;
using KiotVietTimeSheet.Application.EventBus.Events.PayRateTemplateEvents;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.Application.EventBus.Events.PayslipEvents;
using KiotVietTimeSheet.Application.EventBus.Events.PenalizeEvents;
using KiotVietTimeSheet.Application.EventBus.Events.SettingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.ShiftEvents;
using KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents;
using KiotVietTimeSheet.Application.EventBus.Events.AutoKeepingEvents;

namespace KiotVietTimeSheet.AuditTrailWorker.Extensions
{
    public static class AddEventHandlersExtension
    {
        public static void AddEventHandlers(this IServiceCollection services)
        {
            services.AddScoped<IIntegrationEventHandler<CreatedEmployeeIntegrationEvent>, EmployeeIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatedEmployeeIntegrationEvent>, EmployeeIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DeletedEmployeeIntegrationEvent>, EmployeeIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DeletedMultipleEmployeeIntegrationEvent>, EmployeeIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdateEmployeeDeviceIntegrationEvent>, EmployeeIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<AutoTimeKeepingIntegrationEvent>, AutoTimeKeepingIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreateMultipleClockingIntegrationEvent>, ClockingIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdateMultipleClockingIntegrationEvent>, ClockingIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdateAutoKeepingIntegrationEvent>, ClockingIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<RejectMultipleClockingIntegrationEvent>, ClockingIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<SwappedClockingIntegrationEvent>, ClockingIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<ChangedClockingIntegrationEvent>, ClockingIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreatedCommissionDetailByProductIntegrationEvent>, CommissionDetailIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryIntegrationEvent>, CommissionDetailIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryAsyncIntegrationAuditEvent>, CommissionDetailIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DeletedCommissionDetailIntegrationEvent>, CommissionDetailIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatedValueOfCommissionDetailIntegrationEvent>, CommissionDetailIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreatedCommissionDetailByProductIntegrationAuditEvent>, CommissionDetailIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DeletedCommissionDetailIntegrationAuditEvent>, CommissionDetailIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreatedCommissionIntegrationEvent>, CommissionIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatedCommissionIntegrationEvent>, CommissionIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DeletedCommissionIntegrationEvent>, CommissionIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreatedHolidayIntegrationEvent>, HolidayIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatedHolidayIntegrationEvent>, HolidayIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DeletedHolidayIntegrationEvent>, HolidayIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreatedPayRateTemplateIntegrationEvent>, PayRateTemplateIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatedPayRateTemplateIntegrationEvent>, PayRateTemplateIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DeletedPayRateTemplateIntegrationEvent>, PayRateTemplateIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreatedPaysheetIntegrationEvent>, PaysheetIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatedPaysheetIntegrationEvent>, PaysheetIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CancelPaysheetIntegrationEvent>, PaysheetIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CancelPayslipIntegrationEvent>, PayslipIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatedSettingIntegrationEvent>, SettingIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreatedShiftIntegrationEvent>, ShiftIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatedShiftIntegrationEvent>, ShiftIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DeletedShiftIntegrationEvent>, ShiftIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatePaysheetProcessIntegrationEvent>, PaysheetIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatePaysheetProcessErrorIntegrationEvent>, PaysheetIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreatePaysheetProcessIntegrationEvent>, PaysheetIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<CreatedPenalizeIntegrationEvent>, PenalizeIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatedPenalizeIntegrationEvent>, PenalizeIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DeletedPenalizeIntegrationEvent>, PenalizeIntegrationEventHandler>();
            //gpsInfo
            services.AddScoped<IIntegrationEventHandler<CreatedGpsInfoIntegrationEvent>, GpsInfoIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatedGpsInfoIntegrationEvent>, GpsInfoIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DeletedGpsInfoIntegrationEvent>, GpsInfoIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdateSettingClockingGpsIntegrationEvent>, SettingClockingGpsIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<UpdatedQrKeyIntegrationEvent>, GpsInfoIntegrationEventHandler>();
        }
    }
}
