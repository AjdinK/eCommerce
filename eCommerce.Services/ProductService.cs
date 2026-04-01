using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;
using eCommerce.Services.Database;

namespace eCommerce.Services;

public class ProductService : BaseReadService<Product, ProductResponse, ProductSearchObject>, IProductService
{
    // In-memory dummy product collection
    private static readonly List<Product> _dummyProducts = new()
    {
        new Product
        {
            Id = 1,
            Name = "Wireless Mouse",
            Description = "Ergonomic wireless mouse with adjustable DPI.",
            Price = 29.99m,
            StockQuantity = 150,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            SKU = "WM-1001",
            Weight = 85m,
            ProductTypeId = 2,
            UnitOfMeasureId = 1
        },
        new Product
        {
            Id = 2,
            Name = "Mechanical Keyboard",
            Description = "RGB backlit mechanical keyboard with blue switches.",
            Price = 79.99m,
            StockQuantity = 75,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            SKU = "MK-2002",
            Weight = 1200m,
            ProductTypeId = 2,
            UnitOfMeasureId = 1
        },
        new Product
        {
            Id = 3,
            Name = "USB-C Charging Cable",
            Description = "1m braided USB-C to USB-C charging cable.",
            Price = 9.99m,
            StockQuantity = 300,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            SKU = "UC-3003",
            Weight = 50m,
            ProductTypeId = 3,
            UnitOfMeasureId = 1
        }
    };

    public ProductService(MapsterMapper.IMapper mapper) : base(mapper)
    {
    }

    protected override IEnumerable<Product> GetDataSource()
    {
        return _dummyProducts;
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
        IEnumerable<Product> query = GetDataSource();
        query = ApplyFilters(query, search);

        var productWithMaxName = query.OrderByDescending(p => p.Name.Length).First();

        var response = _mapper.Map<ProductResponse>(productWithMaxName);
        return Task.FromResult(response);

    }
}
