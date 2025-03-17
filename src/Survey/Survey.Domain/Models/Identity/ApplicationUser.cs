using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Survey.Domain.Interfaces.Models;
using Survey.Domain.ValueObjects.Identity;

namespace Survey.Domain.Models.Identity
{
    public class ApplicationUser : IdentityUser<Guid>, IAggregateSoftDeletable<Guid>
    {
        [MaxLength(100)]
        [Column("full_name")]
        public required String FullName { get; set; }
        public DateTime? LastEmailConfirmSent { get; set; }
        public DateTime? LastEmailPasswordResetSent { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        // handle domain events
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public IDomainEvent[] ClearDomainEvents()
        {
            IDomainEvent[] dequeuedEvents = _domainEvents.ToArray();
            _domainEvents.Clear();
            return dequeuedEvents;
        }

        // handle refresh tokens
        private readonly List<UserRefreshTokens> _refreshTokens = new();
        public IReadOnlyCollection<UserRefreshTokens> RefreshTokens => _refreshTokens.AsReadOnly();

    }
}
