using Core.Api.Controllers;
using Core.Application.Requests;
using Core.Persistance.Dynamic;
using CustomerService.Api.Attributes;
using CustomerService.Application.Features.Customer.Commands.Create;
using CustomerService.Application.Features.Customer.Commands.Delete;
using CustomerService.Application.Features.Customer.Commands.SaleCreated;
using CustomerService.Application.Features.Customer.Commands.Update;
using CustomerService.Application.Features.Customer.Queries.GetById;
using CustomerService.Application.Features.Customer.Queries.GetList;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CustomerService.Api.Controllers;

[Authorize(Policy = "TokenAuthorizationHandler")]
[EnableRateLimiting("RateLimitUserId")]
[ElasticsearchRequestResponse]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
[Route("api/[controller]")]
[ApiController]
public class CustomersController : BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID([FromRoute] int id)
    {
        GetByIdCustomerQuery getByIdCustomerQuery = new() { Id = id };
        var response = await Mediator.Send(getByIdCustomerQuery);
        return Ok(response);
    }
    [HttpPost("GetList")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest,
        [FromBody] DynamicQuery? dynamicQuery = null)
    {
        GetListCustomerQuery getListCustomerQuery =
            new() { PageRequest = pageRequest, DynamicQuery = dynamicQuery };
        var response = await Mediator.Send(getListCustomerQuery);
        return Ok(response);
    }

    [HttpPost("StartSale")]
    public async Task<IActionResult> StartSale(
        [FromBody] SaleCreatedCommand saleCreatedCommand)
    {
        return Ok(await Mediator.Send(saleCreatedCommand));
    }
    
    /*Rabbit mq istek atıcak*/
    [HttpPost("AddCustomer")]
    public async Task<IActionResult> AddCustomer(
        [FromBody] CreateCustomerCommand createCustomerCommand)
    {
        return Ok(await Mediator.Send(createCustomerCommand));
    }
    /*Rabbit mq istek atıcak*/
    [HttpPut("UpdateCustomer")]
    public async Task<IActionResult> UpdateCustomer(
        [FromBody] UpdateCustomerCommand updateCustomerCommand)
    {
        return Ok(await Mediator.Send(updateCustomerCommand));
    }
    /*Rabbit mq istek atıcak*/
    [HttpDelete()]
    public async Task<IActionResult> DeleteCustomer(
        [FromBody] DeleteCustomerCommand deleteCustomerCommand)
    {
        return Ok(await Mediator.Send(deleteCustomerCommand));
    }
}