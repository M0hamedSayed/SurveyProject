using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Survey.Domain.Abstractions;
using Survey.Domain.ValueObjects.Survey;

namespace Survey.Domain.Models.Survey
{
    [Table("survey_response_answers", Schema = "Survey")]
    public class ResponseAnswer : Entity<Guid>
    {
        [Required]
        public Guid QuestionId { get; private set; }
        [Required]
        public Guid ResponseId { get; private set; }
        [MaxLength(300)]
        public string? TextAnswer { get; private set; }
        public ChoiseSurveyId? ChoiceId { get; private set; }
        public List<ChoiseSurveyId>? MultipleChoices { get; private set; } = new();
        public EvaluateChoiseSurveyId? Evaluation { get; private set; }

        //public virtual SurveyResponse Response { get; private set; }
        private ResponseAnswer() { }

        public ResponseAnswer(Guid id, Guid responseId, Guid questionId, string? textAnswer, ChoiseSurveyId? choiceId, List<ChoiseSurveyId>? multipleChoices, EvaluateChoiseSurveyId? evaluation)
        {
            Id = id;
            QuestionId = questionId;
            ResponseId = responseId;
            QuestionId = questionId;

            if (choiceId is not null)
            {
                ChoiceId = choiceId;
            }
            else if (multipleChoices is not null)
            {
                if (multipleChoices.Count > 10)
                    throw new InvalidOperationException("A question can have a maximum of 10 multiple choices.");

                MultipleChoices = multipleChoices;
            }
            else if (evaluation is not null)
            {
                Evaluation = evaluation;
            }
            else if (!string.IsNullOrWhiteSpace(textAnswer))
            {
                TextAnswer = textAnswer;
            }
            else
            {
                throw new InvalidOperationException("An answer must have either a choice, multiple choices, or an evaluation.");
            }
        }
    }
}
