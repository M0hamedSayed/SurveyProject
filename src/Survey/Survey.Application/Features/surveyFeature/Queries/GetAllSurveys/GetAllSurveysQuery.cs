using MediatR;
using Survey.Application.Base;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Domain.Abstractions;

namespace Survey.Application.Features.surveyFeature.Queries.GetAllSurveys
{
    public record GetAllSurveysQuery : IRequest<Response<PaginatedResult<AddSurveyResult>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SurveyTypeId { get; set; }

        public bool? IsActive { get; set; }
    }
}
