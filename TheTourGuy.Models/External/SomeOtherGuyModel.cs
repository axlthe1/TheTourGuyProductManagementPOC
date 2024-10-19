namespace TheTourGuy.Models.External;

public class SomeOtherGuyModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ProductDescription { get; set; }
    public SomeOtherGuyModel RatingStatistics { get; set; }
    public double Price { get; set; }
    public int DiscountPercentage { get; set; }
    public int Capacity { get; set; }
    public List<string> ImageUrls { get; set; }
}