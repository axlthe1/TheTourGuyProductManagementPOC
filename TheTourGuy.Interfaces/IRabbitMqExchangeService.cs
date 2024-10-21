using TheTourGuy.Models.Internal;

namespace TheTourGuy.Interfaces;

public interface IRabbitMqExchangeService
{
    void Connect();
    Task<IEnumerable<TheTourGuyModel>> SearchExternalProducts(string supplier, ProductFilter filter);
}