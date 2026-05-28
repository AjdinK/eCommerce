
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Services.Database
{
    public class ProductRecommendation
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int RecommendedProductId { get; set; }
        public Product RecommendedProduct { get; set; } = null!;

        [Required]
        public float Score { get; set; }
    }
}
