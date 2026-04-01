using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Model.Exceptions;
using eCommerce.Model.Requests;
using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;
using eCommerce.Services.Database;
using FluentValidation;

namespace eCommerce.Services
{
    public class CategoryService : BaseCRUDService<Category, CategoryResponse, CategorySearchObject, CategoriesInsertRequest, CategoriesUpdateRequest>, ICategoryService
    {
        // dummy in-memory collection with some hierarchical categories
        private static readonly List<Category> _dummyCategories = new()
        {
            new Category
            {
                Id = 1,
                Name = "Electronics",
                Description = "Electronic devices and gadgets.",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new Category
            {
                Id = 2,
                Name = "Computers",
                Description = "Desktops, laptops and accessories.",
                ParentCategoryId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-28)
            },
            new Category
            {
                Id = 3,
                Name = "Smartphones",
                Description = "Mobile phones and accessories.",
                ParentCategoryId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            },
            new Category
            {
                Id = 4,
                Name = "Home",
                Description = "Home and kitchen products.",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            }
        };

        public CategoryService(MapsterMapper.IMapper mapper, IValidator<CategoriesInsertRequest> insertValidator, IValidator<CategoriesUpdateRequest> updateValidator) : base(mapper, insertValidator, updateValidator)
        {
        }

        protected override IEnumerable<Category> GetDataSource()
        {
            return _dummyCategories;
        }

        protected override IList<Category> GetWritableDataSource()
        {
            return _dummyCategories;
        }

        protected override IEnumerable<Category> ApplyFilters(IEnumerable<Category> query, CategorySearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(c => c.Name.Contains(search.Name, StringComparison.OrdinalIgnoreCase));
                }

                if (search.ParentCategoryId.HasValue)
                {
                    query = query.Where(c => c.ParentCategoryId == search.ParentCategoryId.Value);
                }
            }

            return query;
        }

        public Task<CategoryResponse> ExceptionTestingInsertAsync(CategoriesInsertRequest request)
        {
            if (request.Name.Length < 3)
            {
                throw new ClinetException("Category name must be at least 3 characters long.");
            }

            var entity = MapInsertRequestToEntity(request);

            // Set the Id property
            var entityType = entity.GetType();
            var idProperty = entityType.GetProperty("Id");
            idProperty?.SetValue(entity, GenerateNewId());

            // Set CreatedAt if exists
            var createdAtProperty = entityType.GetProperty("CreatedAt");
            if (createdAtProperty?.CanWrite == true)
            {
                createdAtProperty.SetValue(entity, DateTime.UtcNow);
            }

            var dataSource = GetWritableDataSource();
            dataSource.Add(entity);

            return Task.FromResult(_mapper.Map<CategoryResponse>(entity));
        }
    }
}