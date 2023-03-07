using KiotVietTimeSheet.BackgroundTasks.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Linq;

namespace KiotVietTimeSheet.BackgroundTasks.Extensions
{
    public static class AddQuartzExtension
    {        
        public static void AddQuartzJob(this IServiceCollection services, IConfiguration configuration)
        {
            var cfgAutoTimeKeeping = configuration.GetSection("AutoTimeKeeping");
            var isEnableAutoTimeKeeping = cfgAutoTimeKeeping.GetValue<bool>("IsEnable");
            var cfgAutoCreatePaySheet = configuration.GetSection("AutoCreatePaySheet");
            var isEnableAutoCreatePaySheet = cfgAutoCreatePaySheet.GetValue<bool>("IsEnable");
            var isEnableRetryAutoTimeKeeping = isEnableAutoTimeKeeping && !string.IsNullOrWhiteSpace(cfgAutoTimeKeeping.GetValue<string>("ReTrySchedule"));
            
            if (isEnableAutoTimeKeeping)
            {
                //auto keeping                    
                AddAutoKeepingJobJob(services, cfgAutoTimeKeeping);
            }            
            if (isEnableRetryAutoTimeKeeping)
            {
                AddRetryAutoKeepingJob(services, cfgAutoTimeKeeping);
            }           
            if(isEnableAutoTimeKeeping || isEnableRetryAutoTimeKeeping)
            {
                services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            }    
            
        }
        private static void AddAutoKeepingJobJob(IServiceCollection services, IConfigurationSection cfgAutoTimeKeeping)
        {
            services.AddQuartz(q =>
            {
                // Register the job, loading the schedule from configuration
                q.SchedulerId = "Scheduler-Core-AutoKeeping";
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                
                for (int i = 0; i < 50; i++)
                {
                    string group = "0";
                    if (i > 0) group = i.ToString("#");
                    var cronScheduleDefault = GetCronSchedule(cfgAutoTimeKeeping, group);

                    if (IsAllowAutoKeepingGroup(cfgAutoTimeKeeping, group))
                    {
                        string jobName = $"{typeof(AutoKeepingJob).Name}-{group.GetHashCode()}";
                        var jobKey = new JobKey(jobName);

                        q.AddJob<AutoKeepingJob>(opts => opts.WithIdentity(jobKey)
                                .UsingJobData("groupId", group));

                        q.AddTrigger(opts => opts
                                .ForJob(jobKey)
                                .WithIdentity(jobName + "-trigger")
                                .StartNow()
                                .WithCronSchedule(cronScheduleDefault)
                            );
                    }
                }
            });
            services.AddTransient<AutoKeepingJob>();
        }
        
        private static void AddRetryAutoKeepingJob(IServiceCollection services, IConfigurationSection cfgAutoTimeKeeping)
        {
            services.AddQuartz(q =>
            {
                // Register the job, loading the schedule from configuration
                q.SchedulerId = "Scheduler-RetryAutoKeepingJob";
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();               
                for (int i = 0; i < 50; i++)
                {
                    string group = "0";
                    if (i > 0) group = i.ToString("#");
                    var cronScheduleDefault = GetCronReTrySchedule(cfgAutoTimeKeeping, group);

                    if (IsAllowAutoKeepingGroup(cfgAutoTimeKeeping, group))
                    {
                        string jobName = $"{typeof(RetryAutoKeepingJob).Name}-{group.GetHashCode()}";
                        var jobKey = new JobKey(jobName);

                        q.AddJob<RetryAutoKeepingJob>(opts => opts.WithIdentity(jobKey)
                                .UsingJobData("groupId", group));

                        q.AddTrigger(opts => opts
                                .ForJob(jobKey)
                                .WithIdentity(jobName+"trigger")
                                .StartNow()
                                .WithCronSchedule(cronScheduleDefault)
                            );
                    }
                }
            });
            services.AddTransient<RetryAutoKeepingJob>();
        }

        private static string GetCronSchedule(IConfigurationSection cfgAutoTimeKeeping, string group)
        {
            var cronSchedule = cfgAutoTimeKeeping.GetValue<string>($"ScheduleGroup{group}");
            var cronScheduleDefault = cfgAutoTimeKeeping.GetValue<string>($"Schedule");
            if (!string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronScheduleDefault = cronSchedule;
            }
            return cronScheduleDefault;
        }
        private static string GetCronReTrySchedule(IConfigurationSection cfgAutoTimeKeeping, string group)
        {
            var cronSchedule = cfgAutoTimeKeeping.GetValue<string>($"ScheduleGroup{group}");
            var cronScheduleDefault = cfgAutoTimeKeeping.GetValue<string>($"ReTrySchedule");
            if (!string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronScheduleDefault = cronSchedule;
            }
            return cronScheduleDefault;
        }
        private static bool IsAllowAutoKeepingGroup(IConfigurationSection cfgAutoTimeKeeping,string group)
        {
            var groupIdStrs = cfgAutoTimeKeeping.GetValue<string>("GroupIds");

            var groupIds = groupIdStrs?.Split(',');
            var allowGroupId = groupIds == null || groupIds.Length == 0 || (groupIds.Any(x => x == group || group == ""));
            return allowGroupId;
        }

        private static bool IsAllowAutoCreatePaySheetGroup(IConfigurationSection cfgAutoTimeKeeping, string group)
        {
            var groupIdStrs = cfgAutoTimeKeeping.GetValue<string>("GroupIds");

            var groupIds = groupIdStrs?.Split(',');
            var allowGroupId = groupIds == null || groupIds.Length == 0 || (groupIds.Any(x => x == group || group == ""));

            var excludeGroupIdsStrs = cfgAutoTimeKeeping.GetValue<string>("ExcludeGroupIds");

            var groupExcludeIds = excludeGroupIdsStrs?.Split(',');
            allowGroupId = allowGroupId && (groupExcludeIds == null || groupExcludeIds.Length == 0 || !(groupExcludeIds.Any(x => x == group || group == "")));

            return allowGroupId;
        }
    }
}
