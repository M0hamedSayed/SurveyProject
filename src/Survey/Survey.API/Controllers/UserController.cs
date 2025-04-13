using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Survey.API.Base;
using Survey.Application.Features.User.Queries.GetMe;

namespace Survey.API.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class UserController (IMediator mediator) : AppControllerBase(mediator)
    {
        [HttpGet("me")]
        public async Task<IActionResult> GetUser()
        {
            var res = await _mediator.Send(new GetMeQuery());
            return HandleResult(res);
        }
    }
}
