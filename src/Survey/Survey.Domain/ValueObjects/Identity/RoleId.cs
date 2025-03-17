using Survey.Domain.Exceptions;

namespace Survey.Domain.ValueObjects.Identity
{
    public record RoleId
    {
        public Guid Value { get; }
        private RoleId(Guid value) => Value = value;

        public static RoleId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("RoleId cannot be empty.");
            }
            return new RoleId(value);
        }
    }
}
