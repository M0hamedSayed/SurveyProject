using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsterMapper;
using MediatR;
using Survey.Application.Base;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Application.Interfaces;

namespace Survey.Application.Features.surveyFeature.Queries.GetSurveyById
{
    public class GetSurveyByIdHandler (ISurveyService surveyService, IMapper mapper) : ResponseHandler, IRequestHandler<GetSurveyByIdQuery, Response<AddSurveyResult>>
    {
        public async Task<Response<AddSurveyResult>> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
        {
            var survey = await surveyService.GetSurveyById(request.Id);
            if (survey == null) return NotFound<AddSurveyResult>();
            var Result = mapper.Map<AddSurveyResult>(survey);
            return Success<AddSurveyResult>(Result);
        }
    }
}
