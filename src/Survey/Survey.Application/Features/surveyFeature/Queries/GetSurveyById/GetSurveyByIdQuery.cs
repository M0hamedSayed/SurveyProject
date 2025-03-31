using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Survey.Application.Base;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;

namespace Survey.Application.Features.surveyFeature.Queries.GetSurveyById
{
    public record GetSurveyByIdQuery : IRequest<Response<AddSurveyResult>>
    {
        public required Guid Id { get; set; }
    }
}
