using System.ComponentModel.DataAnnotations;

namespace Gifts.Models
{
    public class Gift
    {
        public int GiftId { get; set; }

        [Required(ErrorMessage = "Gift name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Gift name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}
