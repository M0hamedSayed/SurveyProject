using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Survey.API.Base;
using Survey.API.Filters;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Application.Features.surveyFeature.Commands.AddSurveyPhoto;
using Survey.Application.Features.surveyFeature.Commands.UpdateSurvey;
using Survey.Application.Features.surveyFeature.Queries.GetAllSurveys;
using Survey.Application.Features.surveyFeature.Queries.GetAllSurveyTypes;
using Survey.Application.Features.surveyFeature.Queries.GetSurveyById;
using Survey.Application.Features.surveyFeature.Queries.GetSurveyUsers;

namespace Survey.API.Controllers
{
    [Consumes("application/json")]
    public class SurveyController(IMediator mediator, IConfiguration configuration) : AppControllerBase(mediator)
    {

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSurvey([FromBody] AddSurveyCommand command)
        {
            var res = await _mediator.Send(command);
            return HandleResult(res);
        }
        [HttpPost("add-survey-photo")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSurveyPhoto([FromForm] AddSurveyPhotoCommand command)
        {
            var res = await _mediator.Send(command);
            return HandleResult(res);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("get-all")]
        public async Task<IActionResult> GetAllSurvys([FromBody] GetAllSurveysQuery? query)
        {
            query ??= new GetAllSurveysQuery();
            var res = await _mediator.Send(query);
            return HandleResult(res);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("get-all-survey-types")]
        public async Task<IActionResult> GetAllSurveyTypes([FromQuery] GetAllSurveyTypesQuery query)
        {
            var res = await _mediator.Send(query);
            return HandleResult(res);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetSurveyById([FromRoute] GetSurveyByIdQuery query)
        {
            var res = await _mediator.Send(query);
            return HandleResult(res);
        }

        [HttpPost("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSurvey([FromBody] UpdateSurveyCommand command)
        {
            var res = await _mediator.Send(command);
            return HandleResult(res);
        }

        [ApiKey]
        [HttpPost("participatnts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetParticipants([FromBody] GetSurveyUsersQuery query )
        {
            var res = await _mediator.Send(query);
            return HandleResult(res);
        }
    }
}
