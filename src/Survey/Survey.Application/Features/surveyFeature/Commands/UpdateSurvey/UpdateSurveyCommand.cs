using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Survey.Application.Base;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Application.Interfaces;
using Survey.Domain.Enums;

namespace Survey.Application.Features.surveyFeature.Commands.UpdateSurvey
{
    public class UpdateSurveyCommand :IRequest<Response<AddSurveyResult>>
    {
        public required Guid Id { get; set; }
        public required Guid SurveyTypeId { get; set; }
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
        public required List<QuestionSurveyUpdateCommand> Questions { get; set; }
    }

    public record QuestionSurveyUpdateCommand
    {
        public  Guid? Id { get; set; }
        public required string QuestionEn { get; set; }
        public required string QuestionAr { get; set; }
        public QuestionType QuestionType { get; set; }
        public List<ChoiceSurveyUpdateCommand>? Choices { get; set; }
        public List<EvaluateChoiceSurveyUpdateCommand>? EvaluateChoices { get; set; }
    }

    public record ChoiceSurveyUpdateCommand
    {
        public Guid? Id { get; set; }
        public required string TextAr { get; set; }
        public required string TextEn { get; set; }
    }

    public record EvaluateChoiceSurveyUpdateCommand
    {
        public Guid? Id { get; set; }
        public required string TextAr { get; set; }
        public required string TextEn { get; set; }
        public required string Emotion { get; set; }
    }
}
