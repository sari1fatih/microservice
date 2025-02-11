using Core.Api.Controllers;
using Core.Application.Requests;
using Core.Persistance.Dynamic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SaleService.Api.Attributes;
using SaleService.Application.Features.SaleDetails.Commands.Create;
using SaleService.Application.Features.SaleDetails.Queries.GetById;
using SaleService.Application.Features.SaleDetails.Queries.GetList;

namespace SaleService.Api.Controllers;

[Authorize(Policy = "TokenAuthorizationHandler")]
[EnableRateLimiting("RateLimitUserId")]
[ElasticsearchRequestResponse]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
[Route("api/[controller]")]
[ApiController]
public class SaleDetailsController : BaseController
{
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID([FromRoute] int id)
    {
        GetByIdSaleDetailQuery getByIdSaleGroupQuery = new() { Id = id };
        var response = await Mediator.Send(getByIdSaleGroupQuery);
        return Ok(response);
    }
    
    [HttpPost("GetList")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest,[FromQuery] int saleId,
        [FromBody] DynamicQuery? dynamicQuery = null)
    {
        GetListSaleDetailQuery getListByDynamicModelQuery =
            new() { PageRequest = pageRequest, DynamicQuery = dynamicQuery,saleId = saleId};
        var response = await Mediator.Send(getListByDynamicModelQuery);
        return Ok(response);
    }
    
    [HttpPost("AddSaleDetail")]
    public async Task<IActionResult> AddSaleDetail(
        [FromBody] CreateSaleDetailCommand createSaleDetailCommand)
    {
        return Ok(await Mediator.Send(createSaleDetailCommand));
    }
    
}