using Microsoft.AspNetCore.Mvc;
using eCommerce.Services;
using eCommerce.Model.SearchObjects;

namespace eCommerce.WebAPI.Controllers;

/// <summary>
/// Generic base controller for CRUD operations (Create, Read, Update, Delete)
/// </summary>
/// <typeparam name="TResponse">The response model type</typeparam>
/// <typeparam name="TSearch">The search/filter object type</typeparam>
/// <typeparam name="TInsertRequest">The insert request model type</typeparam>
/// <typeparam name="TUpdateRequest">The update request model type</typeparam>
/// <typeparam name="TService">The service interface type implementing CRUD operations</typeparam>
[ApiController]
[Route("[controller]")]
public abstract class BaseCRUDController<TResponse, TSearch, TInsertRequest, TUpdateRequest, TService>
    : BaseReadController<TResponse, TSearch, TService>
    where TSearch : BaseSearchObject
    where TService : IBaseCRUDService<TResponse, TSearch, TInsertRequest, TUpdateRequest>
{
    protected BaseCRUDController(TService service) : base(service)
    {
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TResponse>> Create([FromBody] TInsertRequest request)
    {
        try
        {
            var result = await _service.InsertAsync(request);
            var idValue = result?.GetType().GetProperty("Id")?.GetValue(result);
            return CreatedAtAction(nameof(GetById), new { id = idValue }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TResponse>> Update(int id, [FromBody] TUpdateRequest request)
    {
        try
        {
            // Set the id in the update request
            var idProperty = typeof(TUpdateRequest).GetProperty("Id");
            if (idProperty?.CanWrite == true)
            {
                idProperty.SetValue(request, id);
            }

            var result = await _service.UpdateAsync(request);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
