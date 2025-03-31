using Survey.Domain.Exceptions;

namespace Survey.Domain.ValueObjects.Survey
{
    public record QuestionSurveyId
    {
        public Guid Value { get; }
        private QuestionSurveyId(Guid value) => Value = value;

        public static QuestionSurveyId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("QuestionSurveyId cannot be empty.");
            }
            return new QuestionSurveyId(value);
        }
    }
}
