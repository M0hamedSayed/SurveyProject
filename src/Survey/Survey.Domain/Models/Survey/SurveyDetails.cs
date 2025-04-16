
using Survey.Domain.Enums;

namespace Survey.Domain.Models.Survey
{
    public class SurveyDetails
    {
        public Guid Id { get; set; }
        public string name_ar { get; set; }
        public string name_en { get; set; }
        public string? description_ar { get; set; }
        public string? description_en { get; set; }
        public string? closing_address_ar { get; set; }
        public string? closing_address_en { get; set; }
        public string? closing_statement_ar { get; set; }
        public string? closing_statement_en { get; set; }
        public DateTimeOffset start_date { get; set; }
        public DateTimeOffset end_date { get; set; }
        public bool? reminder_sent { get; set; }
        public string? image_url { get; set; }
        public bool is_active { get; set; }
        public bool is_required { get; set; }
        public Guid user_id { get; set; }
        public string timezone { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        // Survey type
        public Guid SurveyTypeId { get; set; }
        public string SurveyTypeNameAr { get; set; }
        public string SurveyTypeNameEn { get; set; }

        // Question
        public Guid QuestionId { get; set; }
        public string QuestionAr { get; set; }
        public string QuestionEn { get; set; }
        public QuestionType QuestionType { get; set; }

        // Choices (for types 1 and 2)
        public Guid? ChoiceId { get; set; }
        public string? ChoiceTextEn { get; set; }
        public string? ChoiceTextAr { get; set; }

        // Evaluate choices (for type 3)
        public Guid? EvaluateChoiceId { get; set; }
        public string? EvaluateChoiceTextAr { get; set; }
        public string? EvaluateChoiceTextEn { get; set; }
        public string? EvaluateChoiceEmoji { get; set; }
        public int? SurveyCount { get; set; } = 0;
    }

}
