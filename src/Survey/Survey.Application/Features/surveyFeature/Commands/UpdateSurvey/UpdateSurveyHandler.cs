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

namespace Survey.Application.Features.surveyFeature.Commands.UpdateSurvey
{
    public class UpdateSurveyHandler (ISurveyService surveyService, IMapper mapper) : ResponseHandler, IRequestHandler<UpdateSurveyCommand, Response<AddSurveyResult>>
    {
        public async Task<Response<AddSurveyResult>> Handle(UpdateSurveyCommand request, CancellationToken cancellationToken)
        {
            var survey = await surveyService.UpdateSurvey(request);
            if (survey == null) return NotFound<AddSurveyResult>();

            var result = mapper.Map<AddSurveyResult>(survey);
            return Success(result);
        }
    }
}
