using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Collections.Generic;

namespace Normandy.Infrastructure.JobSchedule
{
    /// <summary>
    /// 
    /// </summary>
    public static class JobScheduleServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        public static IServiceCollection AddJobSchedules(this IServiceCollection services, IList<JobConfigs> configs)
        {
            services.Configure<QuartzOptions>(options =>
            {
                options.Scheduling.IgnoreDuplicates = false;
                options.Scheduling.OverWriteExistingData = true;
            });

            foreach (var config in configs)
            {
                if (!config.Switch)
                {
                    continue;
                }

                services.AddQuartz(t =>
                {
                    // handy when part of cluster or you want to otherwise identify multiple schedulers
                    t.SchedulerId = config.Id;

                    t.UseMicrosoftDependencyInjectionJobFactory();

                    // add job
                    t.AddJob(config.Type, new JobKey(config.Id), options =>
                    {
                        options
                        .WithIdentity(config.Id)
                        .WithDescription(config.Description);
                    });

                    // add shedule
                    t.AddTrigger(t =>
                    {
                        t
                        .StartAt(config.StartTimeUtc)
                        .WithCronSchedule(config.Cron)
                        .WithIdentity(config.Id)
                        .WithDescription(config.Description);
                    });
                });
            }

            services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            return services;
        }
    }
}
