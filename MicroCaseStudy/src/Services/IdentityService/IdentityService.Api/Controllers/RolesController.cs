using Core.Api.Controllers;
using Core.Application.Requests;
using Core.Persistance.Dynamic;
using IdentityService.Api.Attributes;
using IdentityService.Application.Features.Roles.Commands.Create;
using IdentityService.Application.Features.Roles.Commands.Delete;
using IdentityService.Application.Features.Roles.Commands.Update;
using IdentityService.Application.Features.Roles.Queries.GetById;
using IdentityService.Application.Features.Roles.Queries.GetList;
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
public class RolesController : BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID([FromRoute] int id)
    {
        GetByIdRoleQuery getByIdRoleQuery = new() { Id = id };
        var response = await Mediator.Send(getByIdRoleQuery);
        return Ok(response);
    }

    [HttpPost("GetList")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest,
        [FromBody] DynamicQuery? dynamicQuery = null)
    {
        GetListRoleQuery getListByDynamicModelQuery =
            new() { PageRequest = pageRequest, DynamicQuery = dynamicQuery };
        var response = await Mediator.Send(getListByDynamicModelQuery);
        return Ok(response);
    }

    [HttpPost("AddRole")]
    public async Task<IActionResult> AddRole(
        [FromBody] CreateRoleCommand createRoleCommand)
    {
        return Ok(await Mediator.Send(createRoleCommand));
    }
    
    [HttpPut("UpdateRole")]
    public async Task<IActionResult> UpdateRole(
        [FromBody] UpdateRoleCommand updateRoleCommand)
    {
        return Ok(await Mediator.Send(updateRoleCommand));
    }
    [HttpDelete()]
    public async Task<IActionResult> DeleteRole(
        [FromBody] DeleteRoleCommand deleteRoleCommand)
    {
        return Ok(await Mediator.Send(deleteRoleCommand));
    }
}