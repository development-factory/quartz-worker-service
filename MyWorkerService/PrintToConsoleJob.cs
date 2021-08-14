using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWorkerService
{
    public class PrintToConsoleJob : IJob
    {
        private readonly ILogger<PrintToConsoleJob> _logger;

        public PrintToConsoleJob(ILogger<PrintToConsoleJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            // Your code to execute at trigger time
            string textToLog = "PrintToConsoleJob was executed";
            _logger.LogInformation(textToLog);
            Console.WriteLine(textToLog);
            return Task.CompletedTask;
        }
    }
}
