using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Survey.Domain.Abstractions;
using Survey.Domain.Enums;
using Survey.Domain.Exceptions;
using Survey.Domain.ValueObjects.Survey;

namespace Survey.Domain.Models.Survey
{
    [Table("survey_questions", Schema = "Survey")]

    public class QuestionSurvey : Entity<Guid>
    {
        [Required]
        public Guid SurveyId { get; private set; }
        [MaxLength(300)]
        [Required]
        public string QuestionEn { get; private set; }
        [MaxLength(300)]
        [Required]
        public string QuestionAr { get; private set; }
        [Required]
        public QuestionType QuestionType { get; private set; }

        private readonly List<ChoiceSurvey> _choices = new ();
        private readonly List<EvaluateChoiceSurvey> _evaluateChoices = new ();
        public IReadOnlyCollection<ChoiceSurvey> Choices => _choices.AsReadOnly();
        public IReadOnlyCollection<EvaluateChoiceSurvey> EvaluateChoices => _evaluateChoices.AsReadOnly();

        //public virtual Surveys Survey { get; private set; }
        private QuestionSurvey() { }

        public static QuestionSurvey Create(Guid id, Guid surveyId, string questionEn, string questionAr, QuestionType? questionType)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(surveyId.ToString());
            ArgumentNullException.ThrowIfNullOrWhiteSpace(questionEn);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(questionAr);

            return new QuestionSurvey
            {
                Id = id,
                QuestionType = questionType ?? QuestionType.Sample,
                QuestionEn = questionEn,
                QuestionAr = questionAr,
                SurveyId = surveyId
            };
        }

        public QuestionSurvey Update(string questionEn, string questionAr, QuestionType? questionType)
        {
            QuestionAr = questionAr;
            QuestionEn = questionEn;
            QuestionType = questionType ?? QuestionType;
            return this;
        }

        public void AddChoice(string textEn, string textAr, string? emotion = null)
        {
            // validation
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textAr);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textEn);
            ValidateSampleChoice();
            if (emotion is null && QuestionType.Equals(QuestionType.Evaluate))
                throw new DomainException("Invalid Question");

            if (_choices.Count + _evaluateChoices.Count >= 10)
                throw new DomainException("A question can have a maximum of 10 choices.");


            if (emotion is null)
                _choices.Add(ChoiceSurvey.Create(ChoiseSurveyId.Of(Guid.NewGuid()), Id, textAr, textEn));
            else
                _evaluateChoices.Add(EvaluateChoiceSurvey.Create(EvaluateChoiseSurveyId.Of(Guid.NewGuid()), Id, textAr, textEn, emotion));


        }

        public void addMapChoice (Guid choiceId,string textEn, string textAr, string? emotion = null)
        {
            // validation
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textAr);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textEn);
            ValidateSampleChoice();
            if (emotion is null && QuestionType.Equals(QuestionType.Evaluate))
                throw new DomainException("Invalid Question");

            if (_choices.Count + _evaluateChoices.Count >= 10)
                throw new DomainException("A question can have a maximum of 10 choices.");


            if (emotion is null)
                _choices.Add(ChoiceSurvey.Create(ChoiseSurveyId.Of(choiceId), Id, textAr, textEn));
            else
                _evaluateChoices.Add(EvaluateChoiceSurvey.Create(EvaluateChoiseSurveyId.Of(choiceId), Id, textAr, textEn, emotion));

        }

        public void UpdateChoice(object choiceId, string textEn, string textAr, string? emotion = null)
        {
            ArgumentNullException.ThrowIfNull(choiceId);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textAr);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textEn);

            var choice = ValidateChoice(choiceId);
            if (choice is ChoiceSurvey)
            {
                (choice as ChoiceSurvey)?.Update(textEn, textAr);
            }
            else
            {
                if (emotion is null)
                    throw new DomainException("emotion is required");
                (choice as EvaluateChoiceSurvey)?.Update(textEn, textAr, emotion);
            }
        }

        public void RemoveChoice(object choiceId)
        {
            var choice = ValidateChoice(choiceId);
            if (choice is ChoiceSurvey)
                _choices.Remove(item: (choice as ChoiceSurvey)!);
            else
                _evaluateChoices.Remove(item: (choice as EvaluateChoiceSurvey)!);
        }

        public void RemoveMultiChoise(List<object> choicesId)
        {
            foreach (var choiceId in choicesId)
            {
                var choice = ValidateChoice(choiceId);
                if (choice is ChoiceSurvey)
                    _choices.Remove(item: (choice as ChoiceSurvey)!);
                else
                    _evaluateChoices.Remove(item: (choice as EvaluateChoiceSurvey)!);
            }
        }

        private object? ValidateChoice(object choiceId)
        {
            ValidateSampleChoice();
            if (choiceId is ChoiseSurveyId || choiceId is EvaluateChoiseSurveyId)
            {
                var choice = choiceId is ChoiseSurveyId ? (object?)_choices.FirstOrDefault(c => c.Id == choiceId as ChoiseSurveyId) : (object?)_evaluateChoices.FirstOrDefault(c => c.Id == choiceId as EvaluateChoiseSurveyId);
                if (choice is null)
                    throw new DomainException("Choice not found");
                return choice;
            }
            else
                throw new DomainException("Invalid choice");
        }

        private void ValidateSampleChoice()
        {
            if (QuestionType is QuestionType.Sample)
                throw new DomainException("Can't Apply choice for sample question");
        }
    }
}
