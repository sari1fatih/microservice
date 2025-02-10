using Core.Api.Controllers;
using Core.Application.Requests;
using Core.Persistance.Dynamic;
using CustomerService.Api.Attributes;
using CustomerService.Application.Features.CustomerNote.Commands.Create;
using CustomerService.Application.Features.CustomerNote.Commands.Delete;
using CustomerService.Application.Features.CustomerNote.Commands.Update;
using CustomerService.Application.Features.CustomerNote.Queries.GetById;
using CustomerService.Application.Features.CustomerNote.Queries.GetList;
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
public class CustomerNotesController : BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID([FromRoute] int id)
    {
        GetByIdCustomerNoteQuery getByIdCustomerNoteQuery = new() { Id = id };
        var response = await Mediator.Send(getByIdCustomerNoteQuery);
        return Ok(response);
    }
    [HttpPost("GetList")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest,
        [FromBody] DynamicQuery? dynamicQuery = null)
    {
        GetListCustomerNoteQuery getListByDynamicModelQuery =
            new() { PageRequest = pageRequest, DynamicQuery = dynamicQuery };
        var response = await Mediator.Send(getListByDynamicModelQuery);
        return Ok(response);
    }
    
    [HttpPost("AddCustomerNote")]
    public async Task<IActionResult> AddCustomerNote(
        [FromBody] CreateCustomerNoteCommand createCustomerNoteCommand)
    {
        return Ok(await Mediator.Send(createCustomerNoteCommand));
    }
    
    [HttpPut("UpdateCustomerNote")]
    public async Task<IActionResult> UpdateCustomerNote(
        [FromBody] UpdateCustomerNoteCommand updateCustomerNoteCommand)
    {
        return Ok(await Mediator.Send(updateCustomerNoteCommand));
    }
    [HttpDelete()]
    public async Task<IActionResult> DeleteRole(
        [FromBody] DeleteCustomerNoteCommand deleteCustomerNoteCommand)
    {
        return Ok(await Mediator.Send(deleteCustomerNoteCommand));
    }
    
}