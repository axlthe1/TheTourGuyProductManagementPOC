using TheTourGuy.Models.Internal;

namespace TheTourGuy.Interfaces;

public interface IProductRepository
{
    void LoadProducts();
    Task<IEnumerable<TheTourGuyModel>> SearchProducts(ProductFilter filter);
}