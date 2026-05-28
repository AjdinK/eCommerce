

namespace eCommerce.Model.Responses
{
    public class ProductRecommendationResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductResponse Product { get; set; }
        public int RecommendedProductId { get; set; }
        public ProductResponse RecommendedProduct { get; set; }
        public float Score { get; set; }
    }
}
