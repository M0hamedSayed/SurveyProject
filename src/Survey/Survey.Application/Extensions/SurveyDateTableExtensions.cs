using System.Data;
using Mapster;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Domain.Enums;
using Survey.Domain.Models.Survey;

namespace Survey.Application.Extensions
{
    public static class SurveyDateTableExtensions
    {

        public static AddSurveyResult? MapToSurveyResult(this List<SurveyDetails> surveyDetails)
        {
            if (!surveyDetails.Any()) return null;

            // Get first survey detail for common properties
            var firstDetail = surveyDetails.First();

            // Map questions with their choices
            var questions = surveyDetails
                .Where(sd => sd.QuestionId != Guid.Empty)
                .GroupBy(sd => sd.QuestionId)
                .Select(g =>
                {
                    var firstQuestion = g.First();
                    var question = firstQuestion.Adapt<QuestionSurveyResult>();

                    // Map choices based on question type
                    question.Choices = (firstQuestion.QuestionType == QuestionType.OneChoice || firstQuestion.QuestionType == QuestionType.MultiChoice)
                        ? g.Where(x => x.ChoiceId.HasValue)
                            .Select(x => x.Adapt<ChoiceSurveyResult>())
                            .DistinctBy(c => c.Id)
                            .ToList()
                        : null;

                    question.EvaluateChoices = firstQuestion.QuestionType == QuestionType.Evaluate
                        ? g.Where(x => x.EvaluateChoiceId.HasValue)
                            .Select(x => x.Adapt<EvaluateChoiceSurveyResult>())
                            .DistinctBy(ec => ec.Id)
                            .ToList()
                        : null;

                    return question;
                })
                .ToList();

            // Map survey and add questions
            var result = firstDetail.Adapt<AddSurveyResult>();
            result.Questions = questions;

            return result;
        }

        public static DataTable ToSurveyDetailsTable(this AddSurveyCommand survey, Guid userId)
        {
            var table = new DataTable();
            table.Columns.Add("survey_type_id", typeof(Guid));
            table.Columns.Add("user_id", typeof(Guid));
            table.Columns.Add("name_ar", typeof(string));
            table.Columns.Add("name_en", typeof(string));
            table.Columns.Add("description_en", typeof(string));
            table.Columns.Add("description_ar", typeof(string));
            table.Columns.Add("closing_address_en", typeof(string));
            table.Columns.Add("closing_address_ar", typeof(string));
            table.Columns.Add("closing_statement_en", typeof(string));
            table.Columns.Add("closing_statement_ar", typeof(string));
            table.Columns.Add("start_date", typeof(DateTimeOffset));
            table.Columns.Add("end_date", typeof(DateTimeOffset));
            table.Columns.Add("timezone", typeof(string));
            table.Columns.Add("image_url", typeof(string));
            table.Columns.Add("is_required", typeof(bool));
            table.Columns.Add("is_active", typeof(bool));

            var row = table.NewRow();

            row["name_ar"] = survey.NameAr;
            row["name_en"] = survey.NameEn;
            row["description_ar"] = string.IsNullOrWhiteSpace(survey.DescriptionAr) ? (object) DBNull.Value : survey.DescriptionAr;
            row["description_en"] = string.IsNullOrWhiteSpace(survey.DescriptionEn) ? (object)DBNull.Value : survey.DescriptionEn;
            row["closing_address_ar"] = string.IsNullOrWhiteSpace(survey.ClosingAddressAr) ? (object)DBNull.Value : survey.ClosingAddressAr;
            row["closing_address_en"] = string.IsNullOrWhiteSpace(survey.ClosingAddressEn) ? (object)DBNull.Value : survey.ClosingAddressEn;
            row["closing_statement_ar"] = string.IsNullOrWhiteSpace(survey.ClosingStatementAr) ? (object)DBNull.Value : survey.ClosingStatementAr;
            row["closing_statement_en"] = string.IsNullOrWhiteSpace(survey.ClosingStatementEn) ? (object)DBNull.Value : survey.ClosingStatementEn;
            row["start_date"] = new DateTimeOffset(survey.StartDate, TimeSpan.Zero);
            row["end_date"] = new DateTimeOffset(survey.EndDate, TimeSpan.Zero);
            row["image_url"] = survey.ImageUrl;
            row["is_active"] = false;
            row["is_required"] = survey.IsRequired;
            row["user_id"] = userId;
            row["timezone"] = survey.Timezone ?? "GMT";
            row["survey_type_id"] = survey.SurveyTypeId;

            table.Rows.Add(row);

            /*table.Rows.Add(
                survey.NameAr,
                survey.NameEn,
                survey.DescriptionAr,
                survey.DescriptionEn,
                survey.ClosingAddressAr,
                survey.ClosingAddressEn,
                survey.ClosingStatementAr,
                survey.ClosingStatementEn,
                new DateTimeOffset(survey.StartDate,TimeSpan.Zero),
                new DateTimeOffset(survey.EndDate, TimeSpan.Zero),
                survey.ImageUrl,
                false,
                survey.IsRequired,
                userId,
                survey.Timezone ?? "GMT",
                survey.SurveyTypeId
            );*/

            return table;
        }

        public static DataTable ToQuestionsTable(this List<QuestionSurveyDto> questions)
        {
            var table = new DataTable();
            table.Columns.Add("QuestionEn", typeof(string)).MaxLength = 300;
            table.Columns.Add("QuestionAr", typeof(string)).MaxLength = 300;
            table.Columns.Add("QuestionType", typeof(int));

            foreach (var question in questions)
            {
                table.Rows.Add(
                    question.QuestionEn,
                    question.QuestionAr,
                    question.QuestionType
                );
            }

            return table;
        }

        public static DataTable ToChoicesTable(this List<QuestionSurveyDto> questions)
        {
            var table = new DataTable();
            table.Columns.Add("QuestionIndex", typeof(int));
            table.Columns.Add("text_ar", typeof(string));
            table.Columns.Add("text_en", typeof(string));

            for (int i = 0; i < questions.Count; i++)
            {
                if (questions[i].QuestionType == QuestionType.OneChoice || questions[i].QuestionType == QuestionType.MultiChoice)
                {
                    var inc = i;
                    inc++;
                    foreach (var choice in questions[i].Choices!)
                    {
                        table.Rows.Add(
                            inc, // QuestionIndex
                            choice.TextAr,
                            choice.TextEn
                        );
                    }
                }
            }

            return table;
        }

        public static DataTable ToEvaluateChoicesTable(this List<QuestionSurveyDto> questions)
        {
            var table = new DataTable();
            table.Columns.Add("QuestionIndex", typeof(int));
            table.Columns.Add("text_ar", typeof(string));
            table.Columns.Add("text_en", typeof(string));
            table.Columns.Add("emotion", typeof(string));

            for (int i = 0; i < questions.Count; i++)
            {
                if (questions[i].QuestionType == QuestionType.Evaluate)
                {
                    var inc = i;
                    inc++;
                    foreach (var evalChoice in questions[i].EvaluateChoices!)
                    {
                        table.Rows.Add(
                            inc, // QuestionIndex
                            evalChoice.TextAr,
                            evalChoice.TextEn,
                            evalChoice.Emotion
                        );
                    }
                }
            }

            return table;
        }
    }
}
