using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TheTourGuy.Interfaces;
using TheTourGuy.Models;
using TheTourGuy.Models.Internal;

namespace SomeOtherGuyWorker.Services;



public class RabbitMqService : IRabbitMqService, IDisposable
{
    const string REQEUST_QUEUE = "RequestQueue";
    const string REPLY_QUEUE = "ReplyQueue";
    public string RequestQueue { get; private set; }
    public string ReplyQueue { get; private set; }
    public bool Configured { get; private set; } = false;
    

    
    private readonly RabbitMqConfiguration _configuration;
    private readonly ILogger<RabbitMqService> _logger;

    private  ConnectionFactory _factory;
    private  IConnection _connection;
    private  IModel _channel;
    private AsyncEventingBasicConsumer _consumer;

    public RabbitMqService(RabbitMqConfiguration configuration, ILogger<RabbitMqService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ConnectExternalSearcher(IExternalRepository repository)
    {
        try
        {
            RequestQueue = $"{repository.SupplierName}{REQEUST_QUEUE}";
            ReplyQueue = $"{repository.SupplierName}{REPLY_QUEUE}";

            _factory = new ConnectionFactory() 
                { HostName = _configuration.Host, UserName = _configuration.User, Password = _configuration.Password, DispatchConsumersAsync = true };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _logger.LogInformation($"Connecting to {RequestQueue}");
            _channel.QueueDeclare(queue: RequestQueue, durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            _logger.LogInformation($"Connecting to {ReplyQueue}");
            _channel.QueueDeclare(queue: ReplyQueue, durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        _logger.LogDebug("Message Received in queue {0}", RequestQueue);
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        var request = JsonConvert.DeserializeObject<ProductFilter>(message);
                        _logger.LogDebug($"Received message: {message}");
                        var products = await repository.GetExternalProducts(request);
                        _logger.LogDebug($"Found {products.Count()} products");
                        var response = JsonConvert.SerializeObject(products);

                        // Send the response back to the reply queue
                        var responseBytes = Encoding.UTF8.GetBytes(response);

                        _channel.BasicPublish(exchange: "", routingKey: ReplyQueue, basicProperties: null,
                            body: responseBytes);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }

                ;

            _channel.BasicConsume(queue: RequestQueue, autoAck: true, consumer: _consumer);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw;
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }
}

