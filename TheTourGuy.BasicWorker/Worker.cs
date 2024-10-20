



namespace TheTourGuy.BasicWorker;
using TheTourGuy.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

   public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IExternalRepository _myService;

        public Worker(ILogger<Worker> logger, IExternalRepository myService)
        {
            _logger = logger;
            _myService = myService;
        }

        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service for {0} running at: {1}",_myService.SupplierName, DateTimeOffset.Now);
            await _myService.Configure();
            await _myService.LoadProductsAsync();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                // Simulate some delay
                await Task.Delay(5000, stoppingToken);  // Task runs every 5 seconds
            }
        }
       
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
