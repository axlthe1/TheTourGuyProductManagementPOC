using Microsoft.Extensions.Logging;
using ProductSearcherApi.Repositories;
using ProductSearcherApi.Services;
using TheTourGuy.Models;
using TheTourGuy.Models.Internal;

namespace TheTourGuy.Tests.Repositories;

[TestFixture]
[TestOf(typeof(ProductRepository))]
public class ProductRepositoryTest
{
    private ProductRepository _productRepository;

    [SetUp]
    public void Setup()
    {
        var rest = new RestAPIConfiguration();
        var rmq = new RabbitMqConfiguration();
        ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger<RabbitMqExchangeService> logger = factory.CreateLogger<RabbitMqExchangeService>();
        ILogger<ProductRepository> logger2 = factory.CreateLogger<ProductRepository>();
        _productRepository= new ProductRepository(rest, rmq,new RabbitMqExchangeService(rmq,logger),logger2);
    }
    
    [Test]
    //[TestCase(10,"Zipline Canopy Adventure",null,null,null,1)]
    //[TestCase(1,null,null,null,60,0)]
    [TestCase(1,"Bangkok Grand Palace and Wat Arun Tour",null,null,null,1)] //external otherguy
    public async Task TryRepository(int guests, string? productName, string? destination, string? supplier, decimal? maxPrice,int expectedCount)
    {
        var element = new ProductFilter()
        {
            Destination = destination,
            Guests = guests,
            Supplier = supplier,
            MaxPrice = maxPrice,
            ProductName = productName
        };
        
        var response = await _productRepository.SearchProducts(element);
        
        Assert.That(expectedCount, Is.EqualTo(response.Count()));
    }
}