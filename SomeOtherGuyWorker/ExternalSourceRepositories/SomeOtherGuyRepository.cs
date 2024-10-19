using AutoMapper;
using Newtonsoft.Json.Linq;
using TheTourGuy.Models.External;
using TheTourGuy.Models.Internal;

namespace SomeOtherGuyWorker.ExternalSourceRepositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class SomeOtherGuyRepository
{
    private readonly IMapper _mapper;
    private  ConnectionFactory _factory;
    private  IConnection _connection;
    private  IModel _channel;
    private EventingBasicConsumer _consumer;

    public SomeOtherGuyRepository(IMapper mapper)
    {
        _mapper = mapper;
    }
    public void Configure()
    {
        _factory = new ConnectionFactory() { HostName = "localhost", UserName = "user", Password = "password" };
        _connection = _factory.CreateConnection();
        
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "SomeOtherGuyRequestQueue", durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: "SomeOtherGuyReplyQueue", durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var request = JsonConvert.DeserializeObject<ProductFilter>(message);
                var products = GetExternalProducts(request);
                var response = JsonConvert.SerializeObject(products);

                // Send the response back to the reply queue
                var replyQueue = ea.BasicProperties.ReplyTo;
                var responseBytes = Encoding.UTF8.GetBytes(response);

                _channel.BasicPublish(exchange: "", routingKey: replyQueue, basicProperties: null, body: responseBytes);
            
        }

        ;

        _channel.BasicConsume(queue: "SomeOtherGuyRequestQueue", autoAck: true, consumer: _consumer);
    }

    private IEnumerable<TheTourGuyModel> GetExternalProducts(ProductFilter? request)
    {
       return LoadProductsFromFile("SomeOtherGuyData.json", "SomeOtherGuyData", "Thailand");
    }
    
    private List<TheTourGuyModel> LoadProductsFromFile(string filePath, string supplierName,string destination)
    {
        
        var json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "JsonSource", filePath));
        JObject jsonObject = JObject.Parse(json);
        var products = jsonObject["Products"]?.ToObject<List<SomeOtherGuyModel>>();
        var remappedProducts = _mapper.Map<List<TheTourGuyModel>>(products);
        // Assign the supplier name and add to the main list
        foreach (var product in remappedProducts)
        {
            product.SupplierName = supplierName;
            product.Destination = destination;
            
        }
        
        return remappedProducts;
    }
}