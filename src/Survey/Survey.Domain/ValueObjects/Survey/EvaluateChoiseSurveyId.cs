using Survey.Domain.Exceptions;

namespace Survey.Domain.ValueObjects.Survey
{
    public record EvaluateChoiseSurveyId
    {
        public Guid Value { get; }
        private EvaluateChoiseSurveyId(Guid value) => Value = value;

        public static EvaluateChoiseSurveyId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("EvaluateChoiseSurveyId cannot be empty.");
            }
            return new EvaluateChoiseSurveyId(value);
        }
    }
}
