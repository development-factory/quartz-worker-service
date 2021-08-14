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
                .UseWindowsService()
                .UseSystemd()
                .ConfigureLogging((hostingContext, config) =>
                {
                    config.AddLog4Net();
                })
                .ConfigureServices((hostContext, services) =>
                {

                    JobKey jobKey = new JobKey("my-job");

                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();
                        q.AddJob<PrintToConsoleJob>(config => config.WithIdentity(jobKey));
                        q.AddTrigger(config => config
                            .WithIdentity("my-job-trigger")
                            .WithCronSchedule("0/5 * * * * ?")
                            .ForJob(jobKey));
                    });
                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
                });
    }
}
