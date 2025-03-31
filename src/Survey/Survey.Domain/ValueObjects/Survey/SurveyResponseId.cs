using Survey.Domain.Exceptions;

namespace Survey.Domain.ValueObjects.Survey
{
    public record SurveyResponseId
    {
        public Guid Value { get; }
        private SurveyResponseId(Guid value) => Value = value;

        public static SurveyResponseId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("SurveyId cannot be empty.");
            }
            return new SurveyResponseId(value);
        }
    }
}
