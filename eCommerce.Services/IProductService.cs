using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;

namespace eCommerce.Services
{
    public interface IProductService : IBaseReadService<ProductResponse, ProductSearchObject>
    {
        Task<ProductResponse> GetWithMaxNameAsync(ProductSearchObject? search = null);
    }
}