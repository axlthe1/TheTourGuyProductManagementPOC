namespace TheTourGuy.Models.Internal;

public class TheTourGuyModel
{
    public string Destination { get; set; }
    public string SupplierName { get; set; }
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public double? AverageRating { get; set; }
    public decimal RegularPrice { get; set; }
    public decimal DiscountPrice { get; set; }
    public int MaximumGuests { get; set; }
    public List<ImageModel> Images { get; set; }
}