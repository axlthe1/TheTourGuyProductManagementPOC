using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TheTourGuy.Models.Internal;

namespace ProductSearcherApi.Repositories;

public class ProductRepository
{
    private readonly List<TheTourGuyModel> _products;

    public ProductRepository()
    {
        _products = new List<TheTourGuyModel>();
        LoadProducts();
    }

    private void LoadProducts()
    {
        // Load products from the three JSON files.
        LoadProductsFromFile("TheTourGuyData.json", "TheTourGuy","Mexico");
        //LoadProductsFromFile("SomeOtherGuy.json", "SomeOtherGuy");
        //LoadProductsFromFile("TheBigGuy.json", "TheBigGuy");
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

    public IEnumerable<TheTourGuyModel> SearchProducts(ProductFilter filter)
    {
        if(filter == null)
            throw new ArgumentNullException(nameof(filter));
        
        var query = _products.Where(p => p.MaximumGuests >= filter.Guests);

        if (!string.IsNullOrEmpty(filter.ProductName))
            query = query.Where(p => p.Title.Contains(filter.ProductName, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(filter.Destination))
            query = query.Where(p => p.Destination.Contains(filter.Destination, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(filter.Supplier))
            query = query.Where(p => p.SupplierName.Equals(filter.Supplier, StringComparison.OrdinalIgnoreCase));

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.DiscountPrice <= filter.MaxPrice.Value);

        var addedProducts  = SearchExternalProducts("SomeOtherGuy",filter);
        var result =  query.ToList();
        result.AddRange(addedProducts);
        return query;
    }

    public IEnumerable<TheTourGuyModel> SearchExternalProducts(string supplier,ProductFilter filter)
    {
        // Use RabbitMQ to communicate with external suppliers
        var externalProducts = new List<TheTourGuyModel>();
        var factory = new ConnectionFactory() { HostName = "localhost", UserName = "user", Password = "password" };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            string queueName = supplier + "RequestQueue";  // Use a dedicated queue for each supplier
            var requestProps = channel.CreateBasicProperties();
            requestProps.ReplyTo = supplier + "ReplyQueue";  // Reply queue to listen for response

            

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(filter));
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: requestProps, body: body);

            // Listen for the response from external supplier
            var consumer = new EventingBasicConsumer(channel);
            bool messageReceived = false;
            consumer.Received += (model, ea) =>
            {
                var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                var products = JsonConvert.DeserializeObject<List<TheTourGuyModel>>(response);
                externalProducts.AddRange(products);
                messageReceived = true;
            };
            channel.BasicConsume(queue: requestProps.ReplyTo, autoAck: true, consumer: consumer);
            int timeWait = 0;
            while (!messageReceived && timeWait < 1000)
            {
                Task.Delay(300).Wait();
                timeWait += 300;
            }
            // Wait for a few seconds for the response to come back
            
        }

        return externalProducts;
    }
}