using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Survey.Application.Base;
using Survey.Domain.Abstractions;

namespace Survey.Application.Features.surveyFeature.Queries.GetAllSurveyTypes
{
    public record GetAllSurveyTypesQuery : IRequest<Response<PaginatedResult<GetAllSurveyTypesResult>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
    }
}
