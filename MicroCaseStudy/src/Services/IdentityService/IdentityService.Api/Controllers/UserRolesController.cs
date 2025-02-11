using Core.Api.Controllers;
using Core.Application.Requests;
using Core.Persistance.Dynamic;
using IdentityService.Api.Attributes;
using IdentityService.Application.Features.UserRoles.Commands.Create;
using IdentityService.Application.Features.UserRoles.Commands.Delete;
using IdentityService.Application.Features.UserRoles.Queries.GetById;
using IdentityService.Application.Features.UserRoles.Queries.GetList;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace IdentityService.Api.Controllers;

[ElasticsearchRequestResponse]
[Authorize(Policy = "TokenAuthorizationHandler")]
[EnableRateLimiting("RateLimitUserId")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
[Route("api/[controller]")]
[ApiController]
public class UserRolesController: BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID([FromRoute] int id)
    {
        GetByIdUserRoleQuery getByIdRoleQuery = new() { Id = id };
        var response = await Mediator.Send(getByIdRoleQuery);
        return Ok(response);
    }

    [HttpPost("GetList")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest,
        [FromBody] DynamicQuery? dynamicQuery = null)
    {
        GetListUserRoleQuery getListByDynamicModelQuery =
            new() { PageRequest = pageRequest, DynamicQuery = dynamicQuery };
        var response = await Mediator.Send(getListByDynamicModelQuery);
        return Ok(response);
    }
    [HttpDelete()]
    public async Task<IActionResult> DeleteRole(
        [FromBody] DeleteUserRoleCommand deleteRoleCommand)
    {
        return Ok(await Mediator.Send(deleteRoleCommand));
    }
    
    [HttpPost("AddUserRole")]
    public async Task<IActionResult> AddUserRole(
        [FromBody] CreateUserRoleCommand createUserRoleCommand)
    {
        return Ok(await Mediator.Send(createUserRoleCommand));
    }
}