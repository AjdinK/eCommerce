using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;
using eCommerce.Services.Database;

namespace eCommerce.Services;

public class ProductService : BaseReadService<Product, ProductResponse, ProductSearchObject>, IProductService
{
    public ProductService(ECommerceDbContext dbContext, MapsterMapper.IMapper mapper) : base(mapper, dbContext)
    {
    }



    protected override IEnumerable<Product> ApplyFilters(IEnumerable<Product> query, ProductSearchObject? search)
    {
        if (search != null)
        {
            if (!string.IsNullOrWhiteSpace(search.Name))
            {
                query = query.Where(p => p.Name.Contains(search.Name, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(search.Description))
            {
                query = query.Where(p => p.Description.Contains(search.Description, StringComparison.OrdinalIgnoreCase));
            }
            if (search.ProductTypeId.HasValue)
            {
                query = query.Where(p => p.ProductTypeId == search.ProductTypeId.Value);
            }
        }

        return query;
    }

    public Task<ProductResponse> GetWithMaxNameAsync(ProductSearchObject? search = null)
    {
        IEnumerable<Product> query =  _dbContext.Set<Product>();
        query = ApplyFilters(query, search);

        var productWithMaxName = query.OrderByDescending(p => p.Name.Length).First();

        var response = _mapper.Map<ProductResponse>(productWithMaxName);
        return Task.FromResult(response);

    }
}