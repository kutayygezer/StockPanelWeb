using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockPanelWeb.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Product Name")]
        [Required]
        public string ProductName { get; set; }
        [DisplayName("Stock Code")]
        public string StockCode { get; set; }
        [DisplayName("Category")]
        [Required]
        public int CategoryId { get; set; }
        [DisplayName("Product Quantity")]
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Product quantity cannot be less than 0.")]
        public int ProductAmount { get; set; }
        [DisplayName("Purchase Price")]
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The Buy Price cannot be less than 0.")]
        public int ProductBuyingPrice { get; set; }
        [DisplayName("Sale Price")]
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Sale Price cannot be less than 0.")]
        public int ProductSellingPrice { get; set; }
        public Category? Category { get; set; }
    }
}
