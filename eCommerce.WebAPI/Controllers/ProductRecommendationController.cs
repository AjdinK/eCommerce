using Azure;
using eCommerce.Model.Access;
using eCommerce.Model.Requests;
using eCommerce.Model.SearchObjects;
using eCommerce.Services;
using eCommerce.WebAPI.Services.AccessManager;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductRecommendationController : Controller
    {
        private readonly IProductRecommendationService _productRecommendationService;

        public ProductRecommendationController(IProductRecommendationService productRecommendationService)
        {
            _productRecommendationService = productRecommendationService;
        }

        [HttpPost("GenerateRecommendations")]
        public virtual async Task<IActionResult> GenerateRecommendations()
        {
            await _productRecommendationService.GenerateRecommendationsAsync();
            return Ok("Recommendations generated successfully.");
        }

        [HttpGet("GetRecommendationsForProduct")]
        public virtual async Task<IActionResult> GetRecommendationsForProduct([FromQuery] ProductRecommendationSearch search)
        {
            var result = await _productRecommendationService.GetRecommendationsForProductAsync(search);
            return Ok(result);
        }

        [HttpDelete("GetRecommendationsForProduct")]
        public virtual async Task<IActionResult> DeleteRecommendations()
        {
            await _productRecommendationService.DeleteOldRecommendations();
            return Ok("Recommendations deleted successfully.");
        }

    }
}
