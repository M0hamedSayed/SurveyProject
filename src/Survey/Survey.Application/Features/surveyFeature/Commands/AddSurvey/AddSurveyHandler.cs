using MapsterMapper;
using MediatR;
using Survey.Application.Base;
using Survey.Application.Extensions;
using Survey.Application.Interfaces;
using Survey.Domain.Models.Survey;

namespace Survey.Application.Features.surveyFeature.Commands.AddSurvey
{
    public class AddSurveyHandler(IMapper mapper, ISurveyService surveyService) : ResponseHandler, IRequestHandler<AddSurveyCommand, Response<AddSurveyResult>>
    {
        public async Task<Response<AddSurveyResult>> Handle(AddSurveyCommand request, CancellationToken cancellationToken)
        {
            /*var survey = await surveyService.AddSurvey(request);
            if (survey == null) BadRequest<AddSurveyResult>("something happen wrong, please try again later");
            var response = mapper.Map<AddSurveyResult>(survey!);
            return Success<AddSurveyResult>(response);*/
            var survey = await surveyService.AddSurveyWithSP(request);
            if (survey == null || !survey.Any()) BadRequest<AddSurveyResult>("something happen wrong, please try again later");

            var result = survey!.MapToSurveyResult();

            return Success<AddSurveyResult>(result!);
        }
    }
}
