using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Survey.Domain.Abstractions;
using Survey.Domain.Models.Identity;
using Survey.Domain.ValueObjects.Survey;

namespace Survey.Domain.Models.Survey
{
    [Table("survey_responses", Schema = "Survey")]
    public class SurveyResponse : Aggregate<Guid>
    {
        [Required]
        public Guid SurveyId { get; private set; }
        [Required]
        public Guid UserId { get; private set; }
        private readonly List<ResponseAnswer> _answers = new();

        public IReadOnlyCollection<ResponseAnswer> Answers => _answers.AsReadOnly();

        public virtual ApplicationUser User {  get; private set; }
        private SurveyResponse() { }

        //public SurveyResponse(SurveyResponseId id, SurveyId surveyId, Guid userId)
        //{
        //    Id = id;
        //    SurveyId = surveyId;
        //    UserId = userId;
        //}

        public static SurveyResponse Create(Guid id, Guid surveyId, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(surveyId);
            ArgumentNullException.ThrowIfNull(userId);

            return new SurveyResponse { Id = id, SurveyId = surveyId, UserId = userId };
        }

        public void AddAnswer(Guid questionId, Guid responseId, string? textAnswer, ChoiseSurveyId? choiceId, List<ChoiseSurveyId>? multipleChoices, EvaluateChoiseSurveyId? evaluation)
        {
            var answer = new ResponseAnswer(
                Guid.NewGuid(),
                responseId,
                questionId,
                textAnswer,
                choiceId,
                multipleChoices,
                evaluation
                );
            _answers.Add(answer);
        }
    }
}
