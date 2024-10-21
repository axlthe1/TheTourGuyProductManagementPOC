using TheTourGuy.Models.Internal;

namespace TheTourGuy.Models.Extensions;

public static class TheTourGuyModelExtensions
{
    public static IEnumerable<TheTourGuyModel> TheTourGuyFilter(this IEnumerable<TheTourGuyModel> products, ProductFilter filter)
    {
        var query = products.Where(p => p.MaximumGuests >= filter.Guests);

        if (!string.IsNullOrEmpty(filter.ProductName))
            query = query.Where(p => p.Title.Contains(filter.ProductName, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(filter.Destination))
            query = query.Where(p => p.Destination.Contains(filter.Destination, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(filter.Supplier))
            query = query.Where(p => p.SupplierName.Equals(filter.Supplier, StringComparison.OrdinalIgnoreCase));

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.DiscountPrice <= filter.MaxPrice.Value);
        
        return query.ToList();
    }
}