using MapsterMapper;
using MediatR;
using Survey.Application.Base;
using Survey.Application.Interfaces;
using Survey.Domain.Abstractions;

namespace Survey.Application.Features.surveyFeature.Queries.GetAllSurveyTypes
{
    public class GetAllSurveyTypesHandler (ISurveyService surveyService, IMapper mapper) : ResponseHandler, IRequestHandler<GetAllSurveyTypesQuery, Response<PaginatedResult<GetAllSurveyTypesResult>>>
    {
        public async Task<Response<PaginatedResult<GetAllSurveyTypesResult>>> Handle(GetAllSurveyTypesQuery request, CancellationToken cancellationToken)
        {
            var result = await surveyService.GetAllSurveytypes(request.Search, request.PageNumber, request.PageSize);
            if (result.surveyTypes == null || result.count == 0) return NotFound<PaginatedResult<GetAllSurveyTypesResult>>();

            var response = mapper.Map<List<GetAllSurveyTypesResult>>(result.surveyTypes!);

            var data = PaginatedResult<GetAllSurveyTypesResult>.Success(response, result.count, request.PageNumber, request.PageSize);
            return Success(data);
        }
    }
}
