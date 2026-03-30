using System.Threading.Tasks;
using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;

namespace eCommerce.Services;

public interface IBaseReadService<TResponse, TSearch>
    where TSearch : BaseSearchObject
{
    Task<TResponse> GetByIdAsync(int id);
    Task<PageResult<TResponse>> GetAllAsync(TSearch? search = null);
}