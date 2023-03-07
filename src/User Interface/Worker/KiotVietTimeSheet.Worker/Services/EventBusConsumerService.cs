using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.Hosting;
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

namespace KiotVietTimeSheet.AuditTrailWorker.Services
{
    public class EventBusConsumerService : BackgroundService
    {
        private readonly IEventBus _eventBus;

        public EventBusConsumerService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _eventBus.Subscribe<CreatedEmployeeIntegrationEvent, IIntegrationEventHandler<CreatedEmployeeIntegrationEvent>>();
            _eventBus.Subscribe<UpdatedEmployeeIntegrationEvent, IIntegrationEventHandler<UpdatedEmployeeIntegrationEvent>>();
            _eventBus.Subscribe<DeletedEmployeeIntegrationEvent, IIntegrationEventHandler<DeletedEmployeeIntegrationEvent>>();
            _eventBus.Subscribe<DeletedMultipleEmployeeIntegrationEvent, IIntegrationEventHandler<DeletedMultipleEmployeeIntegrationEvent>>();
            _eventBus.Subscribe<UpdateEmployeeDeviceIntegrationEvent, IIntegrationEventHandler<UpdateEmployeeDeviceIntegrationEvent>>();
            _eventBus.Subscribe<AutoTimeKeepingIntegrationEvent, IIntegrationEventHandler<AutoTimeKeepingIntegrationEvent>>();
            _eventBus.Subscribe<UpdateAutoKeepingIntegrationEvent, IIntegrationEventHandler<UpdateAutoKeepingIntegrationEvent>>();
            _eventBus.Subscribe<CreateMultipleClockingIntegrationEvent, IIntegrationEventHandler<CreateMultipleClockingIntegrationEvent>>();
            _eventBus.Subscribe<UpdateMultipleClockingIntegrationEvent, IIntegrationEventHandler<UpdateMultipleClockingIntegrationEvent>>();
            _eventBus.Subscribe<RejectMultipleClockingIntegrationEvent, IIntegrationEventHandler<RejectMultipleClockingIntegrationEvent>>();
            _eventBus.Subscribe<SwappedClockingIntegrationEvent, IIntegrationEventHandler<SwappedClockingIntegrationEvent>>();
            _eventBus.Subscribe<ChangedClockingIntegrationEvent, IIntegrationEventHandler<ChangedClockingIntegrationEvent>>();           
            _eventBus.Subscribe<CreatedCommissionDetailByProductIntegrationEvent, IIntegrationEventHandler<CreatedCommissionDetailByProductIntegrationEvent>>();
            _eventBus.Subscribe<CreatedCommissionDetailByProductCategoryIntegrationEvent, IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryIntegrationEvent>>();
            _eventBus.Subscribe<DeletedCommissionDetailIntegrationEvent, IIntegrationEventHandler<DeletedCommissionDetailIntegrationEvent>>();
            _eventBus.Subscribe<UpdatedValueOfCommissionDetailIntegrationEvent, IIntegrationEventHandler<UpdatedValueOfCommissionDetailIntegrationEvent>>();
            _eventBus.Subscribe<CreatedCommissionIntegrationEvent, IIntegrationEventHandler<CreatedCommissionIntegrationEvent>>();
            _eventBus.Subscribe<UpdatedCommissionIntegrationEvent, IIntegrationEventHandler<UpdatedCommissionIntegrationEvent>>();
            _eventBus.Subscribe<DeletedCommissionIntegrationEvent, IIntegrationEventHandler<DeletedCommissionIntegrationEvent>>();
            _eventBus.Subscribe<CreatedHolidayIntegrationEvent, IIntegrationEventHandler<CreatedHolidayIntegrationEvent>>();
            _eventBus.Subscribe<UpdatedHolidayIntegrationEvent, IIntegrationEventHandler<UpdatedHolidayIntegrationEvent>>();
            _eventBus.Subscribe<DeletedHolidayIntegrationEvent, IIntegrationEventHandler<DeletedHolidayIntegrationEvent>>();
            _eventBus.Subscribe<CreatedPayRateTemplateIntegrationEvent, IIntegrationEventHandler<CreatedPayRateTemplateIntegrationEvent>>();
            _eventBus.Subscribe<UpdatedPayRateTemplateIntegrationEvent, IIntegrationEventHandler<UpdatedPayRateTemplateIntegrationEvent>>();
            _eventBus.Subscribe<DeletedPayRateTemplateIntegrationEvent, IIntegrationEventHandler<DeletedPayRateTemplateIntegrationEvent>>();
            _eventBus.Subscribe<CreatedPaysheetIntegrationEvent, IIntegrationEventHandler<CreatedPaysheetIntegrationEvent>>();
            _eventBus.Subscribe<UpdatedPaysheetIntegrationEvent, IIntegrationEventHandler<UpdatedPaysheetIntegrationEvent>>();
            _eventBus.Subscribe<CancelPaysheetIntegrationEvent, IIntegrationEventHandler<CancelPaysheetIntegrationEvent>>();
            _eventBus.Subscribe<CancelPayslipIntegrationEvent, IIntegrationEventHandler<CancelPayslipIntegrationEvent>>();
            _eventBus.Subscribe<UpdatedSettingIntegrationEvent, IIntegrationEventHandler<UpdatedSettingIntegrationEvent>>();
            _eventBus.Subscribe<CreatedShiftIntegrationEvent, IIntegrationEventHandler<CreatedShiftIntegrationEvent>>();
            _eventBus.Subscribe<UpdatedShiftIntegrationEvent, IIntegrationEventHandler<UpdatedShiftIntegrationEvent>>();
            _eventBus.Subscribe<DeletedShiftIntegrationEvent, IIntegrationEventHandler<DeletedShiftIntegrationEvent>>();
            _eventBus.Subscribe<CreatedCommissionDetailByProductIntegrationAuditEvent, IIntegrationEventHandler<CreatedCommissionDetailByProductIntegrationAuditEvent>>();
            _eventBus.Subscribe<DeletedCommissionDetailIntegrationAuditEvent, IIntegrationEventHandler<DeletedCommissionDetailIntegrationAuditEvent>>();
            _eventBus.Subscribe<UpdatePaysheetProcessIntegrationEvent, IIntegrationEventHandler<UpdatePaysheetProcessIntegrationEvent>>();
            _eventBus.Subscribe<UpdatePaysheetProcessErrorIntegrationEvent, IIntegrationEventHandler<UpdatePaysheetProcessErrorIntegrationEvent>>();
            _eventBus.Subscribe<CreatePaysheetProcessIntegrationEvent, IIntegrationEventHandler<CreatePaysheetProcessIntegrationEvent>>();
            _eventBus.Subscribe<CreatedPenalizeIntegrationEvent, IIntegrationEventHandler<CreatedPenalizeIntegrationEvent>>();
            _eventBus.Subscribe<UpdatedPenalizeIntegrationEvent, IIntegrationEventHandler<UpdatedPenalizeIntegrationEvent>>();
            _eventBus.Subscribe<DeletedPenalizeIntegrationEvent, IIntegrationEventHandler<DeletedPenalizeIntegrationEvent>>();
            //GpsInfo
            _eventBus.Subscribe<CreatedGpsInfoIntegrationEvent, IIntegrationEventHandler<CreatedGpsInfoIntegrationEvent>>();
            _eventBus.Subscribe<UpdatedGpsInfoIntegrationEvent, IIntegrationEventHandler<UpdatedGpsInfoIntegrationEvent>>();
            _eventBus.Subscribe<DeletedGpsInfoIntegrationEvent, IIntegrationEventHandler<DeletedGpsInfoIntegrationEvent>>();
            _eventBus.Subscribe<UpdateSettingClockingGpsIntegrationEvent, IIntegrationEventHandler<UpdateSettingClockingGpsIntegrationEvent>>();
            _eventBus.Subscribe<UpdatedQrKeyIntegrationEvent, IIntegrationEventHandler<UpdatedQrKeyIntegrationEvent>>();
            return Task.CompletedTask;
        }
    }
}
