using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MyWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // Run it as a windows service in case the OS is windows
                .UseWindowsService()
                // Run it as background linux service in case the OS is a linux distro
                .UseSystemd()
                .ConfigureLogging((hostingContext, config) =>
                {
                    // Adding log4net as logger
                    config.AddLog4Net();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Unique key for the job
                    JobKey jobKey = new JobKey("my-job");

                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();
                        // Add each job's configuration like this
                        q.AddJob<PrintToConsoleJob>(config => config.WithIdentity(jobKey));
                        // Add each job's trigger like this and bind it to the job by the key
                        q.AddTrigger(config => config
                            .WithIdentity("my-job-trigger")
                            .WithCronSchedule("0/5 * * * * ?")
                            .ForJob(jobKey));
                    });
                    // Let Quartz know that it should finish all the running jobs slowly before shutting down.
                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
                });
    }
}
