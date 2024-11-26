using apl_batch_readQueue.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apl_batch_readQueue.Configuration
{
    public static class QuartzConfiguration
    {
        public static void CreateHostBuilder(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(q =>
            {
                q.AddJobAndTrigger<ExecuteProcessService>(configuration);
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }
    }
}
