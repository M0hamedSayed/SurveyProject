using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Survey.Application.Base;

namespace Survey.API.Base
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AppControllerBase : ControllerBase
    {
        protected readonly IMediator _mediator;

        public AppControllerBase(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected ObjectResult HandleResult<T>(Response<T> result) => result.ToObjectResult();
    }
}
