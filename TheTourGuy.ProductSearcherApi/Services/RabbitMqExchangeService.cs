using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TheTourGuy.Interfaces;
using TheTourGuy.Models;
using TheTourGuy.Models.Internal;

namespace ProductSearcherApi.Services;



public class RabbitMqExchangeService : IRabbitMqExchangeService, IDisposable
{
    private bool _connectionDone = false;
    private object locker = new object();
    private readonly RabbitMqConfiguration _configuration;
    private readonly ILogger<RabbitMqExchangeService> _logger;
    private IConnection _connection;
    const string REQEUST_QUEUE = "RequestQueue";
    const string REPY_QUEUE = "ReplyQueue";

    protected IConnection Connection
    {
        get
        {
            lock (locker)
            {
                return _connection;
            }
        }
    }

    public RabbitMqExchangeService(RabbitMqConfiguration configuration,ILogger<RabbitMqExchangeService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void Connect()
    {
        if (_connectionDone)
            return;
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration.Host, UserName = _configuration.User, Password = _configuration.Password,
                DispatchConsumersAsync = true
            };
            _connection = factory.CreateConnection();
            _connectionDone = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot connect to RabbitMQ");
        }
    }
    
    public async Task<IEnumerable<TheTourGuyModel>> SearchExternalProducts(string supplier, ProductFilter filter)
    {
         // Use RabbitMQ to communicate with external suppliers
        var externalProducts = new List<TheTourGuyModel>();

        Connect();
        try
        {
            using (var channel = Connection.CreateModel())
            {
                string queueName = CreateRequestQueueName(supplier); // Use a dedicated queue for each supplier
                var requestProps = channel.CreateBasicProperties();
                requestProps.ReplyTo = CreateUniqueReplyQueueName(supplier); // Reply queue to listen for response
                requestProps.Expiration = _configuration.TimeoutMilliseconds.ToString();
                requestProps.ContentType = "application/json";
                channel.QueueDeclare(queue: requestProps.ReplyTo, durable: false, exclusive: false,
                    autoDelete: false, arguments: null);



                // Listen for the response from external supplier
                var consumer = new AsyncEventingBasicConsumer(channel);
                bool messageReceived = false;
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        _logger.LogInformation($"Received message: {ea.Body}");
                        var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var products = JsonConvert.DeserializeObject<List<TheTourGuyModel>>(response);
                        externalProducts.AddRange(products);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,ex.Message);
                    }
                    messageReceived = true;
                };


                channel.BasicConsume(queue: requestProps.ReplyTo, autoAck: true, consumer: consumer);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(filter));
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: requestProps, body: body);

                int timeWait = 0;
                while (!messageReceived && timeWait < _configuration.TimeoutMilliseconds)
                {
                    Task.Delay(300).Wait();
                    timeWait += 300;
                }

                channel.QueueDelete(requestProps.ReplyTo);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return externalProducts;
    }

    private static string CreateRequestQueueName(string supplier)
    {
        return supplier + REQEUST_QUEUE;
    }

    private static string CreateUniqueReplyQueueName(string supplier)
    {
        return supplier + REPY_QUEUE + Guid.NewGuid().ToString();
    }

    public void Dispose()
    {
        Connection?.Dispose();
    }
}