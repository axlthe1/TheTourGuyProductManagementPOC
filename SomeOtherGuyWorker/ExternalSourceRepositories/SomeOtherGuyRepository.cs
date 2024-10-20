using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TheTourGuy.Interfaces;
using TheTourGuy.Models.External;
using TheTourGuy.Models.Internal;

namespace SomeOtherGuyWorker.ExternalSourceRepositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class SomeOtherGuyRepository : IExternalRepository
{
    List<TheTourGuyModel> InternalData { get; set; }
    public string SupplierName { get; set; }
    public string Destination { get; set; }
    
    private readonly IMapper _mapper;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ILogger<SomeOtherGuyRepository> _logger;

    public SomeOtherGuyRepository(IMapper mapper,IRabbitMqService rabbitMqService,ILogger<SomeOtherGuyRepository> logger)
    {
        SupplierName = "SomeOtherGuy";
        Destination = "Thailand";
        _mapper = mapper;
        _rabbitMqService = rabbitMqService;
        _logger = logger;
    }
    public async Task Configure()
    {
        await _rabbitMqService.ConnectExternalSearcher(this);
    }



    public async Task<IEnumerable<TheTourGuyModel>> GetExternalProducts(ProductFilter? request)
    {
        if (InternalData == null)
            await LoadProductsAsync();
            
        var query = InternalData.Where(p => p.MaximumGuests >= request.Guests);

        if (!string.IsNullOrEmpty(request.ProductName))
            query = query.Where(p => p.Title.Contains(request.ProductName, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(request.Destination))
            query = query.Where(p => p.Destination.Contains(request.Destination, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(request.Supplier))
            query = query.Where(p => p.SupplierName.Equals(request.Supplier, StringComparison.OrdinalIgnoreCase));

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.DiscountPrice <= request.MaxPrice.Value);
        
        return query.ToList();
        

    }
    
    public List<TheTourGuyModel> LoadFromFileProducts(string filePath, string supplierName,string destination)
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

    public async Task<List<TheTourGuyModel>> LoadProductsAsync()
    {
        try
        {
            InternalData = await Task.Run(() => LoadFromFileProducts("SomeOtherGuyData.json", SupplierName, Destination));
            return InternalData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products");
        }

        return await Task.FromResult<List<TheTourGuyModel>>(null);
    }

    
}

