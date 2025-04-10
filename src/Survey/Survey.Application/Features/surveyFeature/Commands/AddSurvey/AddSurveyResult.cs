using Microsoft.AspNetCore.Http;
using Survey.Domain.Enums;

namespace Survey.Application.Features.surveyFeature.Commands.AddSurvey
{
    public record AddSurveyResult
    {
        public required Guid Id { get; set; }
        public required Guid SurveyTypeId { get; set; }
        public required string SurveyTypeNameEn { get; set; }
        public required string SurveyTypeNameAr { get; set; }

        public required string NameEn { get; set; }
        public required string NameAr { get; set; }

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ClosingAddressEn { get; set; }
        public string? ClosingAddressAr { get; set; }
        public string? ClosingStatementEn { get; set; }
        public string? ClosingStatementAr { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsRequired { get; set; } = false;
        public bool? IsActive { get; set; } = false;
        public string? Timezone { get; set; }
        public required List<QuestionSurveyResult> Questions { get; set; }
    }

    public record QuestionSurveyResult
    {
        public required Guid Id { get; set; }
        public required string QuestionEn { get; set; }
        public required string QuestionAr { get; set; }
        public QuestionType QuestionType { get; set; }
        public List<ChoiceSurveyResult>? Choices { get; set; }
        public List<EvaluateChoiceSurveyResult>? EvaluateChoices { get; set; }
    }

    public record ChoiceSurveyResult
    {
        public required Guid Id { get; set; }
        public required string TextAr { get; set; }
        public required string TextEn { get; set; }
    }

    public record EvaluateChoiceSurveyResult
    {
        public required Guid Id { get; set; }
        public required string TextAr { get; set; }
        public required string TextEn { get; set; }
        public required string Emotion { get; set; }
    }
}
