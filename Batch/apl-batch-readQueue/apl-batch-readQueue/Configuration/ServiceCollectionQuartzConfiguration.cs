using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apl_batch_readQueue.Configuration
{
    public static class ServiceCollectionQuartzConfiguration
    {
        public static void AddJobAndTrigger<T>(this IServiceCollectionQuartzConfigurator quartz, IConfiguration config) where T : IJob
        {
            string jobName = typeof(T).Name;

            var configKey = $"Quartz:{jobName}";
            int intervalInSeconds = 10;

            var jobKey = new JobKey(jobName);
            quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

            quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(jobName + "-trigger")
            .WithSimpleSchedule(s => s.WithIntervalInSeconds(intervalInSeconds).RepeatForever()));
        }
    }
}
