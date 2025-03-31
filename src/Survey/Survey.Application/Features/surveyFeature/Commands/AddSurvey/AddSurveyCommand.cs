using MediatR;
using Microsoft.AspNetCore.Http;
using Survey.Application.Base;
using Survey.Application.Interfaces;
using Survey.Domain.Enums;

namespace Survey.Application.Features.surveyFeature.Commands.AddSurvey
{
    public record AddSurveyCommand : IRequest<Response<AddSurveyResult>>
    {
        public required string SurveyTypeId { get; set; }
        public required string NameEn { get; set; }
        public required string NameAr { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ClosingAddressEn { get; set; }
        public string? ClosingAddressAr { get; set; }
        public string? ClosingStatementEn { get; set; }
        public string? ClosingStatementAr { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsRequired { get; set; } = false;
        public string? Timezone { get; set; }
        public required List<QuestionSurveyDto> Questions { get; set; }
    }

    public record QuestionSurveyDto
    {
        public required string QuestionEn { get; set; }
        public required string QuestionAr { get; set; }
        public QuestionType QuestionType { get; set; }
        public List<ChoiceSurveyDto>? Choices { get; set; }
        public List<EvaluateChoiceSurveyDto>? EvaluateChoices { get; set; }
    }

    public record ChoiceSurveyDto : IChoiceSurveyDto
    {
        public Guid? Id { get; } = null;
        public required string TextAr { get; set; }
        public required string TextEn { get; set; }
    }

    public record EvaluateChoiceSurveyDto : IEvaluateChoiceSurveyDto
    {
        public Guid? Id { get; } = null;
        public required string TextAr { get; set; }
        public required string TextEn { get; set; }
        public required string Emotion { get; set; }
    }
}
