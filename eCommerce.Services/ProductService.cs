using eCommerce.Model.Requests;
using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;
using eCommerce.Services.Database;
using eCommerce.Services.ProductStateMachine;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eCommerce.Services;

public class ProductService : BaseReadService<Product, ProductResponse, ProductSearchObject>, IProductService
{
    protected BaseProductState ProductState { get; }
    public ProductService(ECommerceDbContext dbContext, MapsterMapper.IMapper mapper, BaseProductState productState) : base(mapper, dbContext)
    {
        ProductState = productState;
    }


    protected override Task<IQueryable<Product>> IncludeRelatedEntitiesAsync(ProductSearchObject? search, IQueryable<Product> query = null)
    {
        if (search?.IncludeProductType == true)
        {
            query = query.Include(p => p.ProductType);
        }
        if (search?.IncludeUnitOfMeasure == true)
        {
            query = query.Include(p => p.UnitOfMeasure);
        }
        if (search?.IncludeAssets == true)
        {
            query = query.Include(p => p.Assets);
        }
        return base.IncludeRelatedEntitiesAsync(search, query);
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

            if (!string.IsNullOrWhiteSpace(search.ProductState))
            {
                query = query.Where(p => p.ProductState.Equals(search.ProductState, StringComparison.OrdinalIgnoreCase));
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

    public async Task<ProductResponse> ActivateAsync(int id)
    {
        var entity = await _dbContext.Products.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Product with id {id} not found.");
        }

        var state = ProductState.GetProductState(entity.ProductState);
        return await state.ActivateAsync(id);
    }

    public async Task<List<string>> GetAllowedActionsAsync(int id)
    {
        if (id <= 0)
        {
             var initialState = ProductState.GetProductState(nameof(InitialProductState));
             return initialState.GetAllowedActions();
        }

        var entity = await _dbContext.Products.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Product with id {id} not found.");
        }

        var state = ProductState.GetProductState(entity.ProductState);
        return state.GetAllowedActions();
    }

    public async Task<ProductResponse> DeactivateAsync(int id)
    {
        var entity = await _dbContext.Products.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Product with id {id} not found.");
        }

        var state = ProductState.GetProductState(entity.ProductState);
        return await state.DeactivateAsync(id);
    }

    public Task<ProductResponse> InsertAsync(ProductInsertRequest request)
    {
        var state = ProductState.GetProductState(nameof(InitialProductState));
        return state.InsertAsync(request);
    }

    public async Task<ProductResponse> UpdateAsync(int id, ProductUpdateRequest request)
    {
        var entity = await _dbContext.Products.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Product with id {id} not found.");
        }

        var state = ProductState.GetProductState(entity.ProductState);
        return await state.UpdateAsync(id, request);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbContext.Products.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Product with id {id} not found.");
        }

        var state = ProductState.GetProductState(entity.ProductState);
        await state.DeleteAsync(id);
    }

    public override async Task<ProductResponse> GetByIdAsync(int id)
    {
        var entity = await _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Reviews.Where(r => r.IsApproved))
            .ThenInclude(r => r.User)
            .Include(p => p.ProductType)
            .Include(p => p.UnitOfMeasure)
            .Include(p => p.Assets)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Product with id {id} not found.");
        }

        var response = _mapper.Map<ProductResponse>(entity);
        response.AllowedActions = await GetAllowedActionsAsync(id);

        return response;
    }

    public static List<int> GenerateDistinctRandomNumbers(int selectedNumber, int count)
    {
        if (selectedNumber < 0)
            throw new ArgumentException("Selected number must be 0 or greater.");

        int rangeSize = selectedNumber + 1; // includes 0 and selectedNumber

        if (count > rangeSize)
            throw new ArgumentException("Count cannot be greater than the size of the range.");

        return Enumerable.Range(0, rangeSize)
            .OrderBy(_ => Random.Shared.Next())
            .Take(count)
            .ToList();
    }

    public async Task<List<OrderResponse>> CreateDummyOrdersAsync(int minProducts = 2, int maxProducts = 5)
    {
        // Get all products
        var products = await _dbContext.Products.ToListAsync();

        // Get all customers

        var customerRole = await _dbContext.Roles.FirstAsync(r => r.Name == "Customer");

        var customers = await _dbContext.Users
            .Include(u => u.UserRoles)
            .Where(u => u.UserRoles.Any(ur => ur.RoleId == customerRole.Id)).ToListAsync();

        if (customers.Count == 0)
            throw new Exception("No customers available to place an order.");

        if (products.Count == 0)
            throw new Exception("No products available to order.");

        // Order list to hold all orders
        var orders = new List<Order>();

        foreach (var customer in customers)
        {
            for(int i = 0; i < 5; i++) // add 5 orders per customer
            {
                var order = new Order
                {
                    UserId = customer.Id,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TotalAmount = 0, // will be calculated
                    OrderItems = new List<OrderItem>(),
                    ShippingAddress = "Random Address",
                    ShippingCity = "Random City",
                    ShippingState = "Random State",
                    ShippingZipCode = "00000",
                    ShippingCountry = "Random Country"
                };

                orders.Add(order);

                var rnd = new Random();
                int numProducts = rnd.Next(minProducts, Math.Min(maxProducts, products.Count) + 1);
                var distinctProductIds = GenerateDistinctRandomNumbers(products.Count - 1, numProducts);

                var selectedProducts = products.OrderBy(x => rnd.Next()).Where(p => distinctProductIds.Contains(p.Id)).Take(numProducts).ToList();

                decimal total = 0;
                foreach (var product in selectedProducts)
                {
                    int quantity = rnd.Next(1, 4); // 1-3 items per product
                    decimal unitPrice = product.Price; // assumes Product has Price property
                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        Discount = 0
                    };
                    total += quantity * unitPrice;
                    order.OrderItems.Add(orderItem);
                }
                order.TotalAmount = total;
            }

        }

        await _dbContext.Orders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();
        var result = orders.Select(o => _mapper.Map<OrderResponse>(o)).ToList();
        return result;

    }

    public List<ProductResponse> Recommend(int id)
    {
        var mlContext = new MLContext();

        var orders = _dbContext.Orders
             .Include(o => o.OrderItems).ToList();

        var data = new List<ProductEntry>();

        foreach (var order in orders)
        {
            if(order.OrderItems?.Count > 1)
            {
                var distinctProductIds = order.OrderItems
                    .Select(x => x.ProductId)
                    .Distinct()
                    .ToList();

                distinctProductIds.ForEach(y =>
                {
                    var relatedItems = order.OrderItems.Where(z => z.ProductId != y);

                    foreach (var z in relatedItems)
                    {
                        data.Add(new ProductEntry
                        {
                            ProductId = (uint)y,
                            CoPurchaseProductId = (uint)z.ProductId

                        });
                    }
                });
            }

        }

        var traindata = mlContext.Data.LoadFromEnumerable(data);

        MatrixFactorizationTrainer.Options options = new MatrixFactorizationTrainer.Options();
        options.MatrixColumnIndexColumnName = nameof(ProductEntry.ProductId);
        options.MatrixRowIndexColumnName = nameof(ProductEntry.CoPurchaseProductId);
        options.LabelColumnName = "Label";
        options.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
        options.Alpha = 0.01;
        options.Lambda = 0.025;
        // For better results use the following parameters
        options.NumberOfIterations = 100;
        options.C = 0.00001;

        var estimator = mlContext.Recommendation().Trainers.MatrixFactorization(options);

        var model = estimator.Fit(traindata);

        var products = _dbContext.Products.Where(x => x.Id != id).ToList();

        var predictionResults = new List<(Product, float)>();

        foreach(var product in products)
        {
            var predictionengine = mlContext.Model.CreatePredictionEngine<ProductEntry, CoPurchasePrediction>(model);
            var prediction = predictionengine.Predict(new ProductEntry
            {
                ProductId = (uint)id,
                CoPurchaseProductId = (uint)product.Id
            });
            predictionResults.Add((product, prediction.Score));
        }

        var finalResults = predictionResults
           .OrderByDescending(x => x.Item2)
           .Take(10) // Get top 10 recommendations
           .Select(x => _mapper.Map<ProductResponse>(x.Item1))
           .ToList();

        return finalResults;
    }
}

//public class CoPurchasePrediction
//{
//    public float Score { get; set; }
//}

//class ProductEntry
//{
//    [KeyType(count: 1000)]
//    public uint ProductId { get; set; }
//    [KeyType(count: 1000)]
//    public uint CoPurchaseProductId { get; set; }
//    public float Label { get; set; }
//}