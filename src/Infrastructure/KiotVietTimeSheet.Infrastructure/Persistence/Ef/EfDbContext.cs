using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.UnitOfWork;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations;
using KiotVietTimeSheet.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.Utilities;
using KiotVietTimeSheet.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore.Storage;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.Infrastructure.EventBus.Extensions;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef.Extensions;
using MediatR;
using KiotVietTimeSheet.Infrastructure.EventBus.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef
{
    public class EfDbContext : DbContext, IUnitOfWork
    {
        private readonly IAuthService _authService;
        private readonly ICacheClient _cacheClient;
        private readonly IMediator _mediator;

        private IDbContextTransaction _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction != null;
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        // for builder
        public EfDbContext(DbContextOptions<EfDbContext> options) : base(options) { }
        public EfDbContext(
            DbContextOptions<EfDbContext> options,
            IAuthService authService,
            ICacheClient cacheClient,
            IMediator mediator
        )
            : base(options)
        {
            _authService = authService;
            _cacheClient = cacheClient;
            _mediator = mediator;
        }

        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<BranchSetting> BranchSettings { get; set; }
        public DbSet<Clocking> Clockings { get; set; }
        public DbSet<ClockingHistory> ClockingHistories { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeProfilePicture> EmployeeProfilePictures { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<NationalHoliday> NationalHolidays { get; set; }
        public DbSet<TenantNationalHoliday> TenantNationalHolidays { get; set; }
        public DbSet<TimeSheet> TimeSheets { get; set; }
        public DbSet<TimeSheetShift> TimeSheetShifts { get; set; }
        public DbSet<AuditProcessFailEvent> AuditProcessFailEvents { get; set; }
        public DbSet<PayRate> PayRate { get; set; }
        public DbSet<Paysheet> Paysheet { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<PayslipDetail> PayslipDetails { get; set; }
        public DbSet<PayslipClocking> PayslipClockings { get; set; }
        public DbSet<Allowance> Allowance { get; set; }
        public DbSet<CronSchedule> CronSchedule { get; set; }
        public DbSet<PayRateTemplate> PayRateTemplate { get; set; }
        public DbSet<PayRateDetail> PayRateDetail { get; set; }
        public DbSet<PayRateTemplateDetail> PayRateTemplateDetail { get; set; }
        public DbSet<FingerMachine> FingerMachine { get; set; }
        public DbSet<FingerPrint> FingerPrint { get; set; }
        public DbSet<Commission> Commission { get; set; }
        public DbSet<CommissionDetail> CommissionDetails { get; set; }
        public DbSet<CommissionBranch> CommissionBranches { get; set; }
        public DbSet<EmployeeBranch> EmployeeBranches { get; set; }
        public DbSet<Penalize> Penalizes { get; set; }
        public DbSet<ClockingPenalize> ClockingPenalizes { get; set; }
        public DbSet<PayslipClockingPenalize> PayslipClockingPenalizes { get; set; }
        public DbSet<PayslipPenalize> PayslipPenalizes { get; set; }
        public DbSet<GpsInfo> GpsInfos { get; set; }
        public DbSet<ConfirmClocking> ConfirmClockings { get; set; }
        public DbSet<ConfirmClockingHistory> ConfirmClockingHistories { get; set; }
        public DbSet<IntegrationEventLogEntry2> IntegrationEventLogEntries { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Hi/lo seq builder
            modelBuilder.HasSequence<long>($"{typeof(Shift).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.HasSequence<long>($"{typeof(Holiday).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.HasSequence<long>($"{typeof(Employee).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.HasSequence<long>($"{typeof(EmployeeProfilePicture).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.HasSequence<long>($"{typeof(TimeSheet).Name}Seq").StartsAt(1).IncrementsBy(1000);
            modelBuilder.HasSequence<long>($"{typeof(Clocking).Name}Seq").StartsAt(1).IncrementsBy(8900);
            modelBuilder.HasSequence<long>($"{typeof(JobTitle).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.HasSequence<long>($"{typeof(Department).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.HasSequence<long>($"{typeof(BranchSetting).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.HasSequence<long>($"{typeof(ClockingHistory).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.HasSequence<long>($"{typeof(Settings).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.HasSequence<long>($"{typeof(FingerPrint).Name}Seq").StartsAt(1).IncrementsBy(1000);
            modelBuilder.HasSequence<long>($"{typeof(Penalize).Name}Seq").StartsAt(1).IncrementsBy(1000);
            modelBuilder.HasSequence<long>($"{typeof(ClockingPenalize).Name}Seq").StartsAt(1).IncrementsBy(1000);
            modelBuilder.HasSequence<long>($"{typeof(PayslipClockingPenalize).Name}Seq").StartsAt(1).IncrementsBy(1000);
            modelBuilder.HasSequence<long>($"{typeof(PayslipPenalize).Name}Seq").StartsAt(1).IncrementsBy(1000);
            modelBuilder.HasSequence<long>($"{typeof(GpsInfo).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.HasSequence<long>($"{typeof(ConfirmClocking).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.HasSequence<long>($"{typeof(ConfirmClockingHistory).Name}Seq").StartsAt(1).IncrementsBy(100);

            //model builder
            modelBuilder.ApplyConfiguration(new ShiftEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new HolidayEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeProfilePictureEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TimeSheetEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TimeSheetShiftEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClockingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new JobTitleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BranchSettingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClockingHistoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SettingsEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new NationalHolidayEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TenantNationalHolidayEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FingerMachineEntityConfiguration());
            modelBuilder.ApplyConfiguration(new FingerPrintEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PenalizeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClockingPenalizeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GpsInfoEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ConfirmClockingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ConfirmClockingHistoryEntityTypeConfiguration());

            // Deduction
            modelBuilder.HasSequence<long>($"{typeof(Deduction).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.ApplyConfiguration(new DeductionEntityTypeConfiguration());

            // Allowance
            modelBuilder.HasSequence<long>($"{typeof(Allowance).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.ApplyConfiguration(new AllowanceEntityTypeConfiguration());

            // PayRate
            modelBuilder.HasSequence<long>($"{typeof(PayRate).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.ApplyConfiguration(new PayRateEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PayRateDetailEntityTypeConfiguration());

            // PayRateTemplate
            modelBuilder.HasSequence<long>($"{typeof(PayRateTemplate).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.ApplyConfiguration(new PayRateTemplateEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PayRateTemplateDetailEntityTypeConfiguration());

            // PaySheet
            modelBuilder.HasSequence<long>($"{typeof(Paysheet).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.ApplyConfiguration(new PaysheetEntityTypeConfiguration());

            // PaySlip
            modelBuilder.HasSequence<long>($"{typeof(Payslip).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.ApplyConfiguration(new PayslipEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PayslipDetailEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PayslipClockingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PayslipClockingPenalizeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PayslipPenalizeEntityTypeConfiguration());

            // Commission
            modelBuilder.HasSequence<long>($"{typeof(Commission).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.ApplyConfiguration(new CommissionEntityTypeConfiguration());

            // Commission Detail
            modelBuilder.HasSequence<long>($"{typeof(CommissionDetail).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.ApplyConfiguration(new CommissionDetailEntityTypeConfiguration());

            // Commission Branch
            modelBuilder.HasSequence<long>($"{typeof(CommissionBranch).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.ApplyConfiguration(new CommissionBranchEntityTypeConfiguration());

            // EventBus
            modelBuilder.ApplyConfiguration(new AuditProcessFailEventEntityTypeConfiguration());

            // Commission Branch
            modelBuilder.HasSequence<long>($"{typeof(EmployeeBranch).Name}Seq").StartsAt(1).IncrementsBy(100);
            modelBuilder.ApplyConfiguration(new EmployeeBranchEntityTypeConfiguration());
        }

        public void Commit()
        {
            ChangeTracker.SetShadowAuditProperties(_authService);
            FlushCacheStore();
            _mediator.DispatchDomainEventsAsync(this).GetAwaiter().GetResult();
            SaveChanges();
        }
        public async Task CommitAsync(bool dispatchEvent = true)
        {
            ChangeTracker.SetShadowAuditProperties(_authService);
            FlushCacheStore();
            if (dispatchEvent)
            {
                await _mediator.DispatchDomainEventsAsync(this);
            }
            await this.BulkSaveChangesAsync();

        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = await Database.BeginTransactionAsync();
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current transaction");

            try
            {
                await this.BulkSaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        private void FlushCacheStore()
        {
            var entities = ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.GetType().HasInterface(typeof(ICacheable)))
                .ToList();

            if (!entities.Any()) return;

            var cleanCacheKeys = entities
                .SelectMany(e =>
                {
                    var entity = e.Entity;
                    var entityType = entity.GetType();
                    var relationEntitiesCacheKeys = Assembly.GetAssembly(entityType)
                        .GetTypes()
                        .Where(t => t != entityType &&
                                    t.GetProperties().Any(p => p.PropertyType == entityType) &&
                                    t.HasInterface(typeof(ICacheable)))
                        .SelectMany(t => new List<string>
                        {
                            $@"{CacheKeys.GetEntityCacheKey(_authService.Context.TenantCode, t.Name, "*")}",
                            CacheKeys.GetListEntityCacheKey(_authService.Context.TenantCode, t.Name, "*")
                        })
                        .ToList();
                    var keyObject = $@"{CacheKeys.GetEntityCacheKey(
                        _authService.Context.TenantCode,
                        entityType.Name,
                        entity.GetType().GetProperty("Id")?.GetValue(entity).ToString()
                    )}*";
                    var keyList = CacheKeys.GetListEntityCacheKey(
                        _authService.Context.TenantCode,
                        entityType.Name,
                        "*"
                    );
                    relationEntitiesCacheKeys.AddRange(new List<string> { keyObject, keyList });
                    return relationEntitiesCacheKeys;
                })
                .Where(k => !string.IsNullOrEmpty(k))
                .Distinct()
                .ToArray();

            if (cleanCacheKeys.Any())
                _cacheClient.RemoveByParttern(cleanCacheKeys);
        }

        //KCCTL-1045 insert wrong data
        public async Task CommitBySaveChangesAsync(bool dispatchEvent = true)
        {
            ChangeTracker.SetShadowAuditProperties(_authService);
            FlushCacheStore();
            if (dispatchEvent)
            {
                await _mediator.DispatchDomainEventsAsync(this);
            }
            await this.SaveChangesAsync();
        }
    }
}
