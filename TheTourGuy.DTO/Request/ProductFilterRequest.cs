using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TheTourGuy.DTO.Request
{
    public class ProductFilterRequest
    {
        [Required]
        public int Guests{ get; set; }
        public string? ProductName { get; set; }
        public string? Destination { get; set; }
        public string? Supplier  { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 0;
    }
}