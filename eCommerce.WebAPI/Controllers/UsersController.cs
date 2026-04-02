using eCommerce.Model.Access;
using eCommerce.Model.Requests;
using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;
using eCommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : BaseCRUDController<UserResponse, UserSearch, UserInsertRequest, UserUpdateRequest, IUserService>
{
    public UsersController(IUserService userService) : base(userService)
    {
    }

    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<UserResponse> Login(UserLoginRequest request)
    {
        var result = await _service.LoginAsync(request);
        return result;
    }
}