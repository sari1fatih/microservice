using Core.Api.Controllers;
using Core.Application.Requests;
using Core.Persistance.Dynamic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SaleService.Api.Attributes;
using SaleService.Application.Features.Sales.Queries.GetById;
using SaleService.Application.Features.Sales.Queries.GetList;

namespace SaleService.Api.Controllers;

[Authorize(Policy = "TokenAuthorizationHandler")]
[EnableRateLimiting("RateLimitUserId")]
[ElasticsearchRequestResponse]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
[Route("api/[controller]")]
[ApiController]
public class SalesController : BaseController
{
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID([FromRoute] int id)
    {
        GetByIdSaleQuery getByIdSaleGroupQuery = new() { Id = id };
        var response = await Mediator.Send(getByIdSaleGroupQuery);
        return Ok(response);
    }
    
    [HttpPost("GetList")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest,
        [FromBody] DynamicQuery? dynamicQuery = null)
    {
        GetListSaleQuery getListByDynamicModelQuery =
            new() { PageRequest = pageRequest, DynamicQuery = dynamicQuery };
        var response = await Mediator.Send(getListByDynamicModelQuery);
        return Ok(response);
    }
    
}