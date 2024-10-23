using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SomeOtherGuyWorker.ExternalSourceRepositories;
using TheTourGuy.BasicWorker;
using TheTourGuy.Interfaces;
using TheTourGuy.Models;

namespace SomeOtherGuyWorker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<RabbitMqConfiguration>();
                    services.AddSingleton<IRabbitMqService, RabbitMqService>();
                    services.AddSingleton<IExternalRepository,SomeOtherGuyRepository>(); 
                    services.AddAutoMapper(typeof(Program));
                })
                .Build();
            await host.RunAsync();
        }
    }

    
}