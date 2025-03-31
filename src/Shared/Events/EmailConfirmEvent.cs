namespace Shared.Events
{
    public record EmailConfirmEvent(string Email, Guid UserId, string Token) : DomainEvent
    {
    }
}
