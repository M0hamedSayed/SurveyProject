using Mapster;
using MapsterMapper;
using MediatR;
using Survey.Application.Base;
using Survey.Application.Extensions;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Application.Interfaces;
using Survey.Domain.Abstractions;
using Survey.Domain.Models.Survey;

namespace Survey.Application.Features.surveyFeature.Queries.GetAllSurveys
{
    public class GetAllSurveysHandler (ISurveyService surveyService, IMapper mapper) : ResponseHandler, IRequestHandler<GetAllSurveysQuery, Response<PaginatedResult<AddSurveyResult>>>
    {

        public async Task<Response<PaginatedResult<AddSurveyResult>>> Handle(GetAllSurveysQuery request, CancellationToken cancellationToken)
        {
            /*var result = await surveyService.GetAllSurveys(request);
            //if (result.surveys is null ||  result.surveys.Count == 0)
                //return NotFound<PaginatedResult<AddSurveyResult>>();
            //var response = mapper.Map<AddSurveyResult>(survey!);
            var response = mapper.Map<List<AddSurveyResult>>(result.surveys!);
            var data = PaginatedResult<AddSurveyResult>.Success(response, result.count, request.PageNumber, request.PageSize);
            return Success(data);*/
            var surveyData = await surveyService.GetAllSurveysWithSp(request);

            if(surveyData.surveyDetails == null || !surveyData.surveyDetails.Any()) return NotFound<PaginatedResult<AddSurveyResult>>();

            var response = surveyData.surveyDetails.GroupBy(s => s.Id).Select(gs => gs.ToList().MapToSurveyResult()).ToList();

            if (response == null || !response.Any()) return NotFound<PaginatedResult<AddSurveyResult>>();

            var data = PaginatedResult<AddSurveyResult>.Success(response!, surveyData.count, request.PageNumber, request.PageSize);
            return Success(data);
        }
    }
}
