using MapsterMapper;
using MediatR;
using Survey.Application.Base;
using Survey.Application.Extensions;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Application.Interfaces;

namespace Survey.Application.Features.surveyFeature.Queries.GetSurveyById
{
    public class GetSurveyByIdHandler (ISurveyService surveyService, IMapper mapper) : ResponseHandler, IRequestHandler<GetSurveyByIdQuery, Response<AddSurveyResult>>
    {
        public async Task<Response<AddSurveyResult>> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
        {
            var surveyDetails = await surveyService.GetSurveyByIdWithSP(request.Id);
            
            if (surveyDetails == null || !surveyDetails.Any()) return NotFound<AddSurveyResult>();

            var Result = surveyDetails.MapToSurveyResult();

            //if (survey == null) return NotFound<AddSurveyResult>();
            //var Result = mapper.Map<AddSurveyResult>(survey);
            return Success<AddSurveyResult>(Result!);
        }
    }
}
