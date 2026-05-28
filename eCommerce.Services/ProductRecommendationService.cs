
using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;
using eCommerce.Services.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace eCommerce.Services
{
    public class ProductRecommendationService : IProductRecommendationService
    {

        private readonly ECommerceDbContext _dbContext;
        protected readonly MapsterMapper.IMapper _mapper;

        public ProductRecommendationService(ECommerceDbContext dbContext, MapsterMapper.IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private async Task<List<ProductEntry>> BuildTrainingData()
        {
            var orders = await _dbContext.Orders
           .Include(o => o.OrderItems)
           .ToListAsync();

            var data = new List<ProductEntry>();

            foreach (var order in orders)
            {
                if (order.OrderItems?.Count > 1)
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
                                CoPurchaseProductId = (uint)z.ProductId,
                            });
                        }

                    });
                }
            }

            return data;
        }

        public List<ProductRecommendation> RecommendSingleProduct(int productId, List<Product> allProducts, MLContext mlContext, ITransformer model)
        {
            var recommendedProducts = allProducts.Where(x => x.Id != productId).ToList(); // Filter out the product itself from the list of products to recommend

            var predictionResults = new List<(Product, float)>(); // List to store recommedatin scores for each product to given product

            foreach (var recommendedProduct in recommendedProducts)
            {
                var predictionengine = mlContext.Model.CreatePredictionEngine<ProductEntry, CoPurchasePrediction>(model);
                var prediction = predictionengine.Predict(new ProductEntry
                {
                    ProductId = (uint)productId,
                    CoPurchaseProductId = (uint)recommendedProduct.Id
                });

                predictionResults.Add((recommendedProduct, prediction.Score));
            }

            var finalResults = predictionResults
               .OrderByDescending(x => x.Item2)
               .Take(10) // Get top 10 recommendations
               .Select(x => new ProductRecommendation
               {
                   ProductId = productId,
                   RecommendedProductId = x.Item1.Id,
                   Score = x.Item2
               })
               .ToList();


            return finalResults;
        }

        public async Task SaveRecommendationsForAllProducts(MLContext mlContext, ITransformer model)
        {
            var products = await _dbContext.Products.ToListAsync(); // Fetch all products from the database to avoid multiple database calls

            foreach (var product in products)
            {
                var recommendationEntities = RecommendSingleProduct(product.Id, products, mlContext, model);
                await _dbContext.ProductRecommendations.AddRangeAsync(recommendationEntities);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task GenerateRecommendationsAsync()
        {
            var mlContext = new MLContext();

            var data = await BuildTrainingData();

            var traindata = mlContext.Data.LoadFromEnumerable(data);

            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(ProductEntry.ProductId),
                MatrixRowIndexColumnName = nameof(ProductEntry.CoPurchaseProductId),
                LabelColumnName = "Label",
                LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                Alpha = 0.01,
                Lambda = 0.025,
                NumberOfIterations = 100,
                C = 0.00001
            };

            var estimator = mlContext.Recommendation().Trainers.MatrixFactorization(options);

            var model = estimator.Fit(traindata); // Train the model

            await SaveRecommendationsForAllProducts(mlContext, model);
        }

        public async Task<List<ProductRecommendationResponse>> GetRecommendationsForProductAsync(ProductRecommendationSearch search)
        {
            var recommendationEntities = await _dbContext.ProductRecommendations
                .Include(x => x.RecommendedProduct)
                .Where(x => x.ProductId == search.ProductId)
                .OrderByDescending(x => x.Score)
                .Take(search.NumberOfRecommendations)
                .ToListAsync();

            var recommendations = recommendationEntities.Select(x => _mapper.Map<ProductRecommendationResponse>(x)).ToList();
            return recommendations;
        }

        public async Task DeleteOldRecommendations()
        {
            await _dbContext.ProductRecommendations.ExecuteDeleteAsync();
        }

    }
}

public class CoPurchasePrediction
{
    public float Score { get; set; }
}

class ProductEntry
{
    [KeyType(count: 1000)]
    public uint ProductId { get; set; }
    [KeyType(count: 1000)]
    public uint CoPurchaseProductId { get; set; }
    public float Label { get; set; }
}
