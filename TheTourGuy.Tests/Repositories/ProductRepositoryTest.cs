using ProductSearcherApi.Repositories;
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
        _productRepository= new ProductRepository();
    }
    
    [Test]
    [TestCase(10,"Zipline Canopy Adventure",null,null,null,1)]
    [TestCase(1,null,null,null,60,0)]
    public void TryRepository(int guests, string? productName, string? destination, string? supplier, decimal? maxPrice,int expectedCount)
    {
        var element = new ProductFilter()
        {
            Destination = destination,
            Guests = guests,
            Supplier = supplier,
            MaxPrice = maxPrice,
            ProductName = productName
        };
        
        var response = _productRepository.SearchProducts(element);
        
        Assert.That(expectedCount, Is.EqualTo(response.Count()));
    }
}