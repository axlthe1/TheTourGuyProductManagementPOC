namespace TheTourGuy.Models.Internal;

public class ProductFilter
{
    public int Guests{ get; set; }
    public string ProductName { get; set; }
    public string Destination { get; set; }
    public string Supplier  { get; set; }
    public decimal? MaxPrice { get; set; }
   
}