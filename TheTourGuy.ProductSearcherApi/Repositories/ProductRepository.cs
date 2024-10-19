using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        LoadProductsFromFile("TheTourGuyData.json", "TheTourGuy");
        //LoadProductsFromFile("SomeOtherGuy.json", "SomeOtherGuy");
        //LoadProductsFromFile("TheBigGuy.json", "TheBigGuy");
    }

    private void LoadProductsFromFile(string filePath, string supplierName)
    {
        var json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "JsonSources", filePath));
        JObject jsonObject = JObject.Parse(json);
        var products = jsonObject["data"]?.ToObject<List<TheTourGuyModel>>();

        // Assign the supplier name and add to the main list
        foreach (var product in products)
        {
            product.SupplierName = supplierName;
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

        return query;
    }


}