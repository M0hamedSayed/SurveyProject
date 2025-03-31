using Survey.Domain.Exceptions;

namespace Survey.Domain.ValueObjects.Survey
{
    public record SurveyResponseAnswerId
    {
        public Guid Value { get; }
        private SurveyResponseAnswerId(Guid value) => Value = value;

        public static SurveyResponseAnswerId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("SurveyId cannot be empty.");
            }
            return new SurveyResponseAnswerId(value);
        }
    }
}
