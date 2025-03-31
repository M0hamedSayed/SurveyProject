using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Survey.Domain.Abstractions;
using Survey.Domain.ValueObjects.Survey;

namespace Survey.Domain.Models.Survey
{
    [Table("survey_choises", Schema = "Survey")]
    public class ChoiceSurvey : ChoiseQuestionSurvey<ChoiseSurveyId>
    {


        //public virtual QuestionSurvey QuestionSurvey { get; private set; }
        private ChoiceSurvey() { }

        private ChoiceSurvey(ChoiseSurveyId id, Guid questionId, string textAr, string textEn)
        : base(id, questionId, textAr, textEn)
        {}

        public static ChoiceSurvey Create(ChoiseSurveyId id, Guid questionId, string textAr, string textEn)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(questionId.ToString());
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textAr);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textEn);

            return new ChoiceSurvey(id, questionId, textAr, textEn);
        }
    }

    [Table("survey_evaluate_choises", Schema = "Survey")]
    public class EvaluateChoiceSurvey : ChoiseQuestionSurvey<EvaluateChoiseSurveyId>
    {
        [Required]
        [MaxLength(10)]
        public string Emotion { get; private set; }

        //public virtual QuestionSurvey QuestionSurvey { get; private set; }


        private EvaluateChoiceSurvey() : base() { }

        private EvaluateChoiceSurvey(EvaluateChoiseSurveyId id, Guid questionId, string textAr, string textEn, string emotion)
        : base(id, questionId, textEn, textAr)
        {
            Emotion = emotion;
        }

        public static EvaluateChoiceSurvey Create(EvaluateChoiseSurveyId id, Guid questionId, string textAr, string textEn, string emotion)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(questionId.ToString());
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textAr);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textEn);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(emotion);

            return new EvaluateChoiceSurvey(id, questionId, textAr, textEn, emotion);
        }

        public override void Update(string textEn, string textAr, string emotion)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(emotion);
            Emotion = emotion;

            base.Update(textEn, textAr);
        }
    }
}
