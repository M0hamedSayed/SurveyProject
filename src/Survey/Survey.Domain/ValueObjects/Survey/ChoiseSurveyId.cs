using Survey.Domain.Exceptions;

namespace Survey.Domain.ValueObjects.Survey
{
    public record ChoiseSurveyId
    {
        public Guid Value { get; }
        private ChoiseSurveyId(Guid value) => Value = value;

        public static ChoiseSurveyId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("ChoiseSurveyId cannot be empty.");
            }
            return new ChoiseSurveyId(value);
        }
    }
}
