

namespace SomeOtherGuyWorker.Workers;
using ExternalSourceRepositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

   public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SomeOtherGuyRepository _myService;

        public Worker(ILogger<Worker> logger, SomeOtherGuyRepository myService)
        {
            _logger = logger;
            _myService = myService;
        }

        // The long-running task that starts when the service runs
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service running at: {time}", DateTimeOffset.Now);
            _myService.Configure();
            // Continuously run a background task until cancellation
            while (!stoppingToken.IsCancellationRequested)
            {
                

                // Simulate some delay
                await Task.Delay(5000, stoppingToken);  // Task runs every 5 seconds
            }
        }

        // Optional - if you want to do something during shutdown
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
