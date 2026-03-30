using eCommerce.Model.SearchObjects;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace eCommerce.Services;

public abstract class BaseCRUDService<TEntity, TResponse, TSearch, TInsertRequest, TUpdateRequest>
    : BaseReadService<TEntity, TResponse, TSearch>
    where TEntity : class
    where TSearch : BaseSearchObject
{
    private readonly IValidator<TInsertRequest> _insertValidator;
    private readonly IValidator<TUpdateRequest> _updateValidator;

    protected BaseCRUDService(MapsterMapper.IMapper mapper, IValidator<TInsertRequest> insertValidator,
        IValidator<TUpdateRequest> updateValidator) : base(mapper)
    {
        _insertValidator = insertValidator;
        _updateValidator = updateValidator;
    }

    protected abstract IList<TEntity> GetWritableDataSource();

    protected virtual int GenerateNewId()
    {
        var dataSource = GetWritableDataSource();
        if (dataSource.Count == 0)
            return 1;

        return dataSource.Max(e => (int)e.GetType().GetProperty("Id")?.GetValue(e)!) + 1;
    }

    protected virtual TEntity MapInsertRequestToEntity(TInsertRequest request)
    {
        var entity = _mapper.Map<TEntity>(request ?? throw new ArgumentNullException(nameof(request)));
        return entity;
    }

    protected virtual void MapUpdateRequestToEntity(TUpdateRequest request, TEntity entity)
    {
        _mapper.Map(request, entity);
    }

    public virtual async Task<TResponse> InsertAsync(TInsertRequest request)
    {
        var validationResult = await _insertValidator.ValidateAsync(request);
        if (validationResult.IsValid == false)
        {
            var errors = validationResult.Errors.Select(e => _mapper.Map<ValidationFailure>(e));
            throw new ValidationException(errors);
        }

        var entity = MapInsertRequestToEntity(request);

        // Set the Id property
        var entityType = entity.GetType();
        var idProperty = entityType.GetProperty("Id");
        idProperty?.SetValue(entity, GenerateNewId());

        // Set CreatedAt if exists
        var createdAtProperty = entityType.GetProperty("CreatedAt");
        if (createdAtProperty?.CanWrite == true) createdAtProperty.SetValue(entity, DateTime.UtcNow);

        var dataSource = GetWritableDataSource();
        dataSource.Add(entity);

        return await Task.FromResult(_mapper.Map<TResponse>(entity));
    }

    public virtual async Task<TResponse> UpdateAsync(TUpdateRequest request)
    {
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (validationResult.IsValid == false)
        {
            var errors = validationResult.Errors.Select(e => _mapper.Map<ValidationFailure>(e));
            throw new ValidationException(errors);
        }

        var idProperty = typeof(TUpdateRequest).GetProperty("Id");
        if (idProperty == null)
            throw new InvalidOperationException($"{typeof(TUpdateRequest).Name} must have an Id property.");

        var id = (int)idProperty.GetValue(request)!;
        var dataSource = GetWritableDataSource();
        var entity = dataSource.FirstOrDefault(e => (int)e.GetType().GetProperty("Id")?.GetValue(e)! == id);

        if (entity == null)
            throw new KeyNotFoundException($"{typeof(TEntity).Name} with id {id} not found.");

        MapUpdateRequestToEntity(request, entity);

        // Update the UpdatedAt timestamp
        var updatedAtProperty = entity.GetType().GetProperty("UpdatedAt");
        if (updatedAtProperty?.CanWrite == true) updatedAtProperty.SetValue(entity, DateTime.UtcNow);

        return await Task.FromResult(_mapper.Map<TResponse>(entity));
    }

    public virtual async Task DeleteAsync(int id)
    {
        var dataSource = GetWritableDataSource();
        var entity = dataSource.FirstOrDefault(e => (int)e.GetType().GetProperty("Id")?.GetValue(e)! == id);

        if (entity == null)
            throw new KeyNotFoundException($"{typeof(TEntity).Name} with id {id} not found.");

        dataSource.Remove(entity);
        await Task.CompletedTask;
    }
}