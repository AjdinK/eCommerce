
using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;

namespace eCommerce.Services
{
    public interface IProductRecommendationService
    {
        Task GenerateRecommendationsAsync();
        Task<List<ProductRecommendationResponse>> GetRecommendationsForProductAsync(ProductRecommendationSearch search);
        Task DeleteOldRecommendations();
    }
}
