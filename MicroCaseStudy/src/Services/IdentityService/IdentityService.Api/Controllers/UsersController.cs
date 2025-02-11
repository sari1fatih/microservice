using Core.Api.Controllers;
using Core.Application.Requests;
using Core.Persistance.Dynamic;
using IdentityService.Api.Attributes;
using IdentityService.Application.Features.Users.Commands.Update;
using IdentityService.Application.Features.Users.Queries.GetById;
using IdentityService.Application.Features.Users.Queries.GetList;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace IdentityService.Api.Controllers
{
    [ElasticsearchRequestResponse]
    [Authorize(Policy = "TokenAuthorizationHandler")]
    [EnableRateLimiting("RateLimitUserId")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID([FromRoute] int id)
        {
            GetByIdUserQuery getByIdSentenceQuery = new() { Id = id };
            var response = await Mediator.Send(getByIdSentenceQuery);
            return Ok(response);
        }

        [HttpPost("GetList")]
        public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest,
            [FromBody] DynamicQuery? dynamicQuery = null)
        {
            GetListUserQuery getListByDynamicModelQuery =
                new() { PageRequest = pageRequest, DynamicQuery = dynamicQuery };
            var response = await Mediator.Send(getListByDynamicModelQuery);
            return Ok(response);
        }
  
        [HttpPut()]
        public async Task<IActionResult> UpdateByID([FromBody] UpdateUserCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }
    }
}