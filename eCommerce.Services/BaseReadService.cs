using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Services
{
    public abstract class BaseReadService<TEntity, TResponse, TSearch> : IBaseReadService<TResponse, TSearch>
        where TEntity : class
        where TSearch : BaseSearchObject
    {
        protected readonly MapsterMapper.IMapper _mapper;

        protected BaseReadService(MapsterMapper.IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the in-memory data source for this entity type.
        /// </summary>
        protected abstract IEnumerable<TEntity> GetDataSource();

        /// <summary>
        /// Applies search filters to the query. Override in derived classes to implement specific filtering logic.
        /// </summary>
        protected abstract IEnumerable<TEntity> ApplyFilters(IEnumerable<TEntity> query, TSearch? search);

        public async Task<PageResult<TResponse>> GetAllAsync(TSearch? search = null)
        {
            IEnumerable<TEntity> query = GetDataSource();
            query = ApplyFilters(query, search);

            int? totalCount = null;

            if (search.IncludeTotalCount ?? false)
            {
                totalCount = query.Count();
            }

            if (search.Page.HasValue)
            {
                query = query.Skip((search.Page.Value - 1) * search.PageSize.Value);
            }

            if (search.PageSize.HasValue)
            {
                query = query.Take(search.PageSize.Value);
            }

            var list = query.Select(item => _mapper.Map<TResponse>(item)).ToList();

            var pageResult = new PageResult<TResponse>
            {
                Items = list,
                TotalCount = totalCount
            };

            return await Task.FromResult(pageResult);
        }

        public async Task<TResponse> GetByIdAsync(int id)
        {
            var entity = GetDataSource().FirstOrDefault(e => (int)e.GetType().GetProperty("Id")?.GetValue(e)! == id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with id {id} not found.");
            }

            return await Task.FromResult(_mapper.Map<TResponse>(entity));
        }
    }
}
