using System;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Funq;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.AutoTimeKeepingEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.ScheduleJobs;
using Quartz;
using Quartz.Impl;
using ServiceStack;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionEvents;
using KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents;
using KiotVietTimeSheet.Application.EventBus.Events.HolidayEvents;
using KiotVietTimeSheet.Application.EventBus.Events.PayrateEvents;
using KiotVietTimeSheet.Application.EventBus.Events.PayRateTemplateEvents;
using KiotVietTimeSheet.Application.EventBus.Events.ShiftEvents;
using KiotVietTimeSheet.Infrastructure.EventBus;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.DomainEventProcessWorker.Configuration;
using KiotVietTimeSheet.DomainEventProcessWorker.Configuration.Types;
using KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler;
using KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.Events;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using Microsoft.EntityFrameworkCore;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.Application.EventBus.Events.PayslipEvents;
using KiotVietTimeSheet.Application.EventBus.Events.RetailerInformationEvents;
using KiotVietTimeSheet.Application.EventBus.Events.SendMailEvents;
using KiotVietTimeSheet.Application.EventBus.Events.SettingEvents;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.DomainEventProcessWorker.KvProcessFailEvent;
using KiotVietTimeSheet.DomainEventProcessWorker.Persistence;
using KiotVietTimeSheet.Infrastructure.Caching.Redis;
using KiotVietTimeSheet.Infrastructure.EventBus.EventLog.EntityFrameworkCore;
using KiotVietTimeSheet.Infrastructure.EventBus.Services;
using KiotVietTimeSheet.SharedKernel.EventBus;
using ServiceStack.Redis;
using CheckUpdateTenantNationalHolidayIntegrationEvent = KiotVietTimeSheet.Application.EventBus.Events.HolidayEvents.CheckUpdateTenantNationalHolidayIntegrationEvent;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.DomainEventProcessWorker
{
    public class AppHost : AppSelfHostBase
    {
        public AppHost() : base("KiotVietTimeSheet.DomainEventProcessWorker", typeof(Program).Assembly) { }
        public override void Configure(Container container)
        {
            container.AddSingleton(c => new HttpClient());

            container.Register<IWorkerConfiguration>(c => new ServiceStackWorkerConfiguration(AppSettings))
                .ReusedWithin(ReuseScope.Container); // singleton

            ConfigEventBus(container);

            ConfigAuditTrailProcess(container);

            ConfigDbAndRepository(container);

            ConfigQuartz();

            container.AddTransient<IKvProcessFailEventService, KvProcessFailEventService>();
        }

        private static void ConfigDbAndRepository(Container container)
        {
            var config = container.Resolve<IWorkerConfiguration>();
            var redisCache = RedisClientsManager.GetClientsManager(config.RedisConfiguration);
            container.AddSingleton(sc => redisCache);
            container.AddSingleton<Application.Caching.ICacheClient>(sp => new RedisClientCache(sp.GetRequiredService<IRedisClientsManager>(), sp.GetRequiredService<ILogger<RedisClientCache>>()));

            container.AddTransient(c =>
            {
                var options = new DbContextOptionsBuilder<EfDbContext>()
                    .UseSqlServer(config.KvTimeSheetConnectionString);
                return new EfDbContext(options.Options);
            });

            container.AddTransient(c =>
            {
                var options = new DbContextOptionsBuilder<KvMasterDataDbContext>()
                    .UseSqlServer(config.KvMasterConnectionString);
                return new KvMasterDataDbContext(options.Options);
            });
        }

        private static void ConfigAuditTrailProcess(Container container)
        {
            var config = container.Resolve<IWorkerConfiguration>().KiotVietServiceClientConfiguration;
            container.AddSingleton<IKiotVietApiClient>(c => new KiotVietApiClient(new KiotVietApiClientContext
            {
                Retail = new KvSystemContext()
                {
                    EndPoint = config.RetailEndPoint,
                    GroupIds = config.RetailGroupIds,
                    InternalToken = config.RetailerInternalToken
                },
                Fnb = new KvSystemContext()
                {
                    EndPoint = config.FnbEndPoint,
                    GroupIds = config.FnbGroupIds,
                    InternalToken = config.FnBInternalToken,
                    KvVersion = config.KvVersion
                }
            }));
            container.AddSingleton<IKiotVietServiceClient>(c => new KiotVietApiClient(new KiotVietApiClientContext
            {
                Retail = new KvSystemContext()
                {
                    EndPoint = config.RetailEndPoint,
                    GroupIds = config.RetailGroupIds,
                    InternalToken = config.RetailerInternalToken
                },
                Fnb = new KvSystemContext()
                {
                    EndPoint = config.FnbEndPoint,
                    GroupIds = config.FnbGroupIds,
                    InternalToken = config.FnBInternalToken,
                    KvVersion = config.KvVersion
                }
            }));
            container.AddTransient<IAuditProcessFailEventService, AuditProcessFailEventService>();
            container.AddSingleton<CommissionDetailAuditProcess>();
        }

        private static void ConfigEventBus(Container container)
        {
            var config = container.Resolve<IWorkerConfiguration>().EventBusConfiguration;
            container.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            container.AddSingleton<IRabbitMqPersistentConnection>(sp =>
            {
                var logger = sp.Resolve<ILogger<RabbitMqPersistentConnection>>();
                var factory = new ConnectionFactory()
                {
                    HostName = config.Connection,
                    Port = config.Port,
                    RequestedHeartbeat = config.RequestedHeartbeat ?? 10
                };

                if (!string.IsNullOrEmpty(config.VirtualHost))
                {
                    factory.VirtualHost = config.VirtualHost;
                }

                if (!string.IsNullOrEmpty(config.UserName))
                {
                    factory.UserName = config.UserName;
                }

                if (!string.IsNullOrEmpty(config.Password))
                {
                    factory.Password = config.Password;
                }
                var retryCount = 5;
                if (config.RetryCount.HasValue)
                {
                    retryCount = config.RetryCount.Value;
                }
                return new RabbitMqPersistentConnection(factory, logger, retryCount);
            });
            container.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
            {
                var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
                var iocContainer = sp.Resolve<IIocContainer>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                var retryCount = 5;
                if (config.RetryCount.HasValue)
                {
                    retryCount = config.RetryCount.Value;
                }

                return new EventBusRabbitMq(
                    rabbitMqPersistentConnection,
                    logger,
                    eventBusSubcriptionsManager,
                    sp,
                    config.QueueName,
                    retryCount
                );
            });

            var eventTypes = Assembly.GetAssembly(typeof(CreatedShiftIntegrationEvent))
                .GetTypes()
                .Where(t => t.BaseType != null && t.BaseType == typeof(IntegrationEvent))
                .ToList();

            container.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(sp => c => new EfIntegrationEventLogService(c, eventTypes));
            container.AddTransient<ITimeSheetIntegrationEventService, TimeSheetIntegrationEventService>(sp =>
                {
                    var eBus = container.GetRequiredService<IEventBus>();
                    var context = container.GetRequiredService<EfDbContext>();
                    var factory = container.GetRequiredService<Func<DbConnection, IIntegrationEventLogService>>();
                    return new TimeSheetIntegrationEventService(eBus, context, factory);
                }
            );

            // subscriber
            var eventBus = container.GetRequiredService<IEventBus>();
            SubscribeEventBus(eventBus);

            // handler
            AddTransientContainer(container);
        }

        private static void ConfigQuartz()
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();

            scheduler.Start().GetAwaiter().GetResult();

            var fetchDomainEventJob = JobBuilder.Create<IntegrationEventRelayJob>().Build();
            var fetchDomainEventJobTrigger = TriggerBuilder
                .Create()
                .StartNow()
                .WithCronSchedule("0/15 * * ? * *")
                .Build();

            var retryKvProcessFailEventJob = JobBuilder.Create<RetryKvProcessFailEventJob>().Build();
            var fetchRetryKvProcessFailEventJobTrigger = TriggerBuilder
                .Create()
                .StartNow()
                .WithCronSchedule("0/15 * * ? * *")
                .Build();

            scheduler.ScheduleJob(fetchDomainEventJob, fetchDomainEventJobTrigger).GetAwaiter().GetResult();
            scheduler.ScheduleJob(retryKvProcessFailEventJob, fetchRetryKvProcessFailEventJobTrigger).GetAwaiter().GetResult();
        }

        public static void SubscribeEventBus(IEventBus eventBus)
        {
            // Shift
            eventBus.Subscribe<CreatedShiftIntegrationEvent, IIntegrationEventHandler<CreatedShiftIntegrationEvent>>();
            eventBus.Subscribe<UpdatedShiftIntegrationEvent, IIntegrationEventHandler<UpdatedShiftIntegrationEvent>>();
            eventBus.Subscribe<DeletedShiftIntegrationEvent, IIntegrationEventHandler<DeletedShiftIntegrationEvent>>();

            // Holiday
            eventBus.Subscribe<CreatedHolidayIntegrationEvent, IIntegrationEventHandler<CreatedHolidayIntegrationEvent>>();
            eventBus.Subscribe<UpdatedHolidayIntegrationEvent, IIntegrationEventHandler<UpdatedHolidayIntegrationEvent>>();
            eventBus.Subscribe<DeletedHolidayIntegrationEvent, IIntegrationEventHandler<DeletedHolidayIntegrationEvent>>();
            eventBus.Subscribe<CheckUpdateTenantNationalHolidayIntegrationEvent, IIntegrationEventHandler<CheckUpdateTenantNationalHolidayIntegrationEvent>>();

            // Clocking
            eventBus.Subscribe<CreatedClockingIntegrationEvent, IIntegrationEventHandler<CreatedClockingIntegrationEvent>>();
            eventBus.Subscribe<UpdatedClockingIntegrationEvent, IIntegrationEventHandler<UpdatedClockingIntegrationEvent>>();
            eventBus.Subscribe<DeletedClockingIntegrationEvent, IIntegrationEventHandler<DeletedClockingIntegrationEvent>>();
            eventBus.Subscribe<RejectedClockingIntegrationEvent, IIntegrationEventHandler<RejectedClockingIntegrationEvent>>();
            eventBus.Subscribe<SwappedClockingIntegrationEvent, IIntegrationEventHandler<SwappedClockingIntegrationEvent>>();
            eventBus.Subscribe<CreateMultipleClockingIntegrationEvent, IIntegrationEventHandler<CreateMultipleClockingIntegrationEvent>>();
            eventBus.Subscribe<UpdateMultipleClockingIntegrationEvent, IIntegrationEventHandler<UpdateMultipleClockingIntegrationEvent>>();
            eventBus.Subscribe<RejectMultipleClockingIntegrationEvent, IIntegrationEventHandler<RejectMultipleClockingIntegrationEvent>>();

            // Employee
            eventBus.Subscribe<CreatedEmployeeIntegrationEvent, IIntegrationEventHandler<CreatedEmployeeIntegrationEvent>>();
            eventBus.Subscribe<UpdatedEmployeeIntegrationEvent, IIntegrationEventHandler<UpdatedEmployeeIntegrationEvent>>();
            eventBus.Subscribe<DeletedEmployeeIntegrationEvent, IIntegrationEventHandler<DeletedEmployeeIntegrationEvent>>();
            eventBus.Subscribe<DeletedMultipleEmployeeIntegrationEvent, IIntegrationEventHandler<DeletedMultipleEmployeeIntegrationEvent>>();

            // Payrate
            eventBus.Subscribe<CreatedPayrateIntegrationEvent, IIntegrationEventHandler<CreatedPayrateIntegrationEvent>>();

            // PayslipsPayment
            eventBus.Subscribe<CreatedPayslipPaymentIntegrationEvent, IIntegrationEventHandler<CreatedPayslipPaymentIntegrationEvent>>();
            eventBus.Subscribe<VoidedPayslipPaymentIntegrationEvent, IIntegrationEventHandler<VoidedPayslipPaymentIntegrationEvent>>();

            //Paysheet
            eventBus.Subscribe<CreatedPaysheetIntegrationEvent, IIntegrationEventHandler<CreatedPaysheetIntegrationEvent>>();
            eventBus.Subscribe<UpdatedPaysheetIntegrationEvent, IIntegrationEventHandler<UpdatedPaysheetIntegrationEvent>>();
            eventBus.Subscribe<CancelPaysheetIntegrationEvent, IIntegrationEventHandler<CancelPaysheetIntegrationEvent>>();

            //Payslip
            eventBus.Subscribe<CancelPayslipIntegrationEvent, IIntegrationEventHandler<CancelPayslipIntegrationEvent>>();

            //Auto timekeeping
            eventBus.Subscribe<AutoTimeKeepingIntegrationEvent, IIntegrationEventHandler<AutoTimeKeepingIntegrationEvent>>();

            // Retry fail events
            eventBus.Subscribe<RabbitPublishFailIntegrationEvent, IIntegrationEventHandler<RabbitPublishFailIntegrationEvent>>();

            // Email
            eventBus.Subscribe<SentMailIntegrationEvent, IIntegrationEventHandler<SentMailIntegrationEvent>>();

            // Commission
            eventBus.Subscribe<CreatedCommissionIntegrationEvent, IIntegrationEventHandler<CreatedCommissionIntegrationEvent>>();
            eventBus.Subscribe<UpdatedCommissionIntegrationEvent, IIntegrationEventHandler<UpdatedCommissionIntegrationEvent>>();
            eventBus.Subscribe<DeletedCommissionIntegrationEvent, IIntegrationEventHandler<DeletedCommissionIntegrationEvent>>();

            // Commission detail
            eventBus.Subscribe<CreatedCommissionDetailByProductIntegrationEvent, IIntegrationEventHandler<CreatedCommissionDetailByProductIntegrationEvent>>();
            eventBus.Subscribe<CreatedCommissionDetailByProductCategoryIntegrationEvent, IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryIntegrationEvent>>();
            eventBus.Subscribe<CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent, IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent>>();
            eventBus.Subscribe<DeletedCommissionDetailIntegrationEvent, IIntegrationEventHandler<DeletedCommissionDetailIntegrationEvent>>();
            eventBus.Subscribe<UpdatedValueOfCommissionDetailIntegrationEvent, IIntegrationEventHandler<UpdatedValueOfCommissionDetailIntegrationEvent>>();

            // PayRate template
            eventBus.Subscribe<CreatedPayRateTemplateIntegrationEvent, IIntegrationEventHandler<CreatedPayRateTemplateIntegrationEvent>>();
            eventBus.Subscribe<DeletedPayRateTemplateIntegrationEvent, IIntegrationEventHandler<DeletedPayRateTemplateIntegrationEvent>>();
            eventBus.Subscribe<UpdatedPayRateTemplateIntegrationEvent, IIntegrationEventHandler<UpdatedPayRateTemplateIntegrationEvent>>();

            // actived feature integration event
            eventBus
                .Subscribe<ActivedFeatureIntegrationEvent, IIntegrationEventHandler<ActivedFeatureIntegrationEvent>>();

            //Setting
            eventBus.Subscribe<UpdatedSettingIntegrationEvent, IIntegrationEventHandler<UpdatedSettingIntegrationEvent>>();

            //Retailer information
            eventBus.Subscribe<RetailerInformationIntegrationEvent, IIntegrationEventHandler<RetailerInformationIntegrationEvent>>();
        }

        public static void AddTransientContainer(Container container)
        {
            // Shift
            container
                .AddTransient<IIntegrationEventHandler<CreatedShiftIntegrationEvent>, ShiftIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<UpdatedShiftIntegrationEvent>, ShiftIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<DeletedShiftIntegrationEvent>, ShiftIntegrationEventHandler>();

            // Holiday
            container
                .AddTransient<IIntegrationEventHandler<CreatedHolidayIntegrationEvent>, HolidayIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<UpdatedHolidayIntegrationEvent>, HolidayIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<DeletedHolidayIntegrationEvent>, HolidayIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<CheckUpdateTenantNationalHolidayIntegrationEvent>, HolidayIntegrationEventHandler>();

            // Clocking
            container
                .AddTransient<IIntegrationEventHandler<SwappedClockingIntegrationEvent>, ClockingIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<CreateMultipleClockingIntegrationEvent>, ClockingIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<UpdateMultipleClockingIntegrationEvent>, ClockingIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<RejectMultipleClockingIntegrationEvent>, ClockingIntegrationEventHandler>();

            // Employee
            container
                .AddTransient<IIntegrationEventHandler<CreatedEmployeeIntegrationEvent>, EmployeeIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<UpdatedEmployeeIntegrationEvent>, EmployeeIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<DeletedEmployeeIntegrationEvent>, EmployeeIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<DeletedMultipleEmployeeIntegrationEvent>, EmployeeIntegrationEventHandler>();

            // PayslipsPayment
            container
                .AddTransient<IIntegrationEventHandler<CreatedPayslipPaymentIntegrationEvent>,
                    PayslipPaymentIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<VoidedPayslipPaymentIntegrationEvent>,
                    PayslipPaymentIntegrationEventHandler>();

            // Paysheet
            container
                .AddTransient<IIntegrationEventHandler<CreatedPaysheetIntegrationEvent>, PaysheetIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<UpdatedPaysheetIntegrationEvent>, PaysheetIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<CancelPaysheetIntegrationEvent>, PaysheetIntegrationEventHandler>();

            // Payslip
            container
                .AddTransient<IIntegrationEventHandler<CancelPayslipIntegrationEvent>, PayslipIntegrationEventHandler>();

            // Auto timeKeeping
            container
                .AddTransient<IIntegrationEventHandler<AutoTimeKeepingIntegrationEvent>, AutoTimeKeepingIntegrationEventHandler>();

            // Retry fail events
            container.AddTransient<IIntegrationEventHandler<RabbitPublishFailIntegrationEvent>, RabbitPublishFailEventHandler>();

            // Email
            container
                .AddTransient<IIntegrationEventHandler<SentMailIntegrationEvent>, EmailIntegrationEventHandler>();

            // Commission
            container
                .AddTransient<IIntegrationEventHandler<CreatedCommissionIntegrationEvent>, CommissionIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<UpdatedCommissionIntegrationEvent>, CommissionIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<DeletedCommissionIntegrationEvent>, CommissionIntegrationEventHandler>();

            // Commission detail
            container.AddTransient<IIntegrationEventHandler<CreatedCommissionDetailByProductIntegrationEvent>, CommissionDetailIntegrationEventHandler>();
            container.AddTransient<IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryIntegrationEvent>, CommissionDetailIntegrationEventHandler>();
            container.AddTransient<IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent>, CommissionDetailIntegrationEventHandler>();
            container.AddTransient<IIntegrationEventHandler<DeletedCommissionDetailIntegrationEvent>, CommissionDetailIntegrationEventHandler>();
            container.AddTransient<IIntegrationEventHandler<UpdatedValueOfCommissionDetailIntegrationEvent>, CommissionDetailIntegrationEventHandler>();

            // PayRate template
            container
                .AddTransient<IIntegrationEventHandler<CreatedPayRateTemplateIntegrationEvent>, PayRateTemplateIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<DeletedPayRateTemplateIntegrationEvent>, PayRateTemplateIntegrationEventHandler>()
                .AddTransient<IIntegrationEventHandler<UpdatedPayRateTemplateIntegrationEvent>, PayRateTemplateIntegrationEventHandler>();

            // actived feature integration event
            container
                .AddTransient<IIntegrationEventHandler<ActivedFeatureIntegrationEvent>, ActivedFeatureIntegrationEventHandler>();

            // Setting
            container
                .AddTransient<IIntegrationEventHandler<UpdatedSettingIntegrationEvent>,
                    SettingIntegrationEventHandler>();

            container
                .AddTransient<IIntegrationEventHandler<RetailerInformationIntegrationEvent>,
                    SendRetailerInformationIntegrationEventHandler>();
        }
    }
}
