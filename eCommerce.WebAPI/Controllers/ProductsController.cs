using eCommerce.Services;
using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class ProductsController : BaseReadController<ProductResponse, ProductSearchObject, IProductService>
{
    public ProductsController(IProductService productService) : base(productService)
    {
    }

    [HttpGet("MaxName")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetWithMaxName([FromQuery] ProductSearchObject? search)
    {
        var result = await _service.GetWithMaxNameAsync(search);
        return Ok(result);
    }
}