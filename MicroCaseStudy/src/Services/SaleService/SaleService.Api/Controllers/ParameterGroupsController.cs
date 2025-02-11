using Core.Api.Controllers;
using Core.Application.Requests;
using Core.Persistance.Dynamic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SaleService.Api.Attributes;
using SaleService.Application.Features.ParameterGroups.Queries.GetById;
using SaleService.Application.Features.ParameterGroups.Queries.GetList;

namespace SaleService.Api.Controllers;

[Authorize(Policy = "TokenAuthorizationHandler")]
[EnableRateLimiting("RateLimitUserId")]
[ElasticsearchRequestResponse]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
[Route("api/[controller]")]
[ApiController]
public class ParameterGroupsController : BaseController
{
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID([FromRoute] int id)
    {
        GetByIdParameterGroupQuery getByIdParameterGroupQuery = new() { Id = id };
        var response = await Mediator.Send(getByIdParameterGroupQuery);
        return Ok(response);
    }
    [HttpPost("GetList")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest,
        [FromBody] DynamicQuery? dynamicQuery = null)
    {
        GetListParameterGroupQuery getListByDynamicModelQuery =
            new() { PageRequest = pageRequest, DynamicQuery = dynamicQuery };
        var response = await Mediator.Send(getListByDynamicModelQuery);
        return Ok(response);
    }
}