using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TheTourGuy.Models;
using TheTourGuy.Models.Extensions;
using TheTourGuy.Models.Internal;

namespace ProductSearcherApi.Repositories;

public class ProductRepository
{
    private readonly RabbitMqConfiguration _configuration;
    private readonly List<TheTourGuyModel> _products;

    public ProductRepository(RabbitMqConfiguration configuration)
    {
        _configuration = configuration;
        _products = new List<TheTourGuyModel>();
        LoadProducts();
    }

    private void LoadProducts()
    {
        LoadProductsFromFile("TheTourGuyData.json", "TheTourGuy","Mexico");
    }

    private void LoadProductsFromFile(string filePath, string supplierName,string destination)
    {
        var json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "JsonSources", filePath));
        JObject jsonObject = JObject.Parse(json);
        var products = jsonObject["data"]?.ToObject<List<TheTourGuyModel>>();

        // Assign the supplier name and add to the main list
        foreach (var product in products)
        {
            product.SupplierName = supplierName;
            product.Destination = destination;
            _products.Add(product);
        }
    }

    public async Task<IEnumerable<TheTourGuyModel>> SearchProducts(ProductFilter filter)
    {
        if(filter == null)
            throw new ArgumentNullException(nameof(filter));
        
        var query = _products.TheTourGuyFilter(filter);

       
        var result =  query.ToList();
        var addedProducts  = await SearchExternalProducts("SomeOtherGuy",filter);
        result.AddRange(addedProducts);
        addedProducts  = await SearchExternalProducts("TheBigGuy",filter);
        result.AddRange(addedProducts);
        return result;
    }

    public async Task<IEnumerable<TheTourGuyModel>> SearchExternalProducts(string supplier,ProductFilter filter)
    {
        // Use RabbitMQ to communicate with external suppliers
        var externalProducts = new List<TheTourGuyModel>();
        var factory = new ConnectionFactory() { HostName = _configuration.Host, UserName = _configuration.User, Password = _configuration.Password ,DispatchConsumersAsync = true };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            string queueName = supplier + "RequestQueue";  // Use a dedicated queue for each supplier
            var requestProps = channel.CreateBasicProperties();
            requestProps.ReplyTo = supplier + "ReplyQueue_" + Guid.NewGuid().ToString();  // Reply queue to listen for response
            requestProps.Expiration = _configuration.TimeoutMilliseconds.ToString();
            requestProps.ContentType = "application/json";
            requestProps.DeliveryMode = 2;
            channel.QueueDeclare(queue: requestProps.ReplyTo, durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            

            
            // Listen for the response from external supplier
            var consumer = new AsyncEventingBasicConsumer(channel);
            bool messageReceived = false;
            consumer.Received += async (model, ea) =>
            {
                var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                var products = JsonConvert.DeserializeObject<List<TheTourGuyModel>>(response);
                externalProducts.AddRange(products);
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
        
        return externalProducts;
    }
}