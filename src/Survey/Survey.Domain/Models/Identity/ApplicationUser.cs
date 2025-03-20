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
        public String FullName { get; private set; }
        public DateTime? LastEmailConfirmSent { get; private set; }
        public DateTime? LastEmailPasswordResetSent { get; private set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; } = null;
        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

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

        // handle self-referencing relationship
        public Guid? ManagerId { get; private set; }
        public ApplicationUser? Manager { get; private set; }

        private readonly List<ApplicationUser> _users = new();
        public IReadOnlyCollection<ApplicationUser> Users => _users.AsReadOnly();

        // handle refresh tokens
        private readonly List<UserRefreshTokens> _refreshTokens = new();
        public IReadOnlyCollection<UserRefreshTokens> RefreshTokens => _refreshTokens.AsReadOnly();

        public ApplicationUser() { }
        public ApplicationUser( string fullName, string email)
        {
            Id = Guid.NewGuid();
            FullName = fullName;
            Email = email;
            UserName = email;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsDeleted = false;
            DeletedAt = null;
        }

        public void HandleSendEmailDate(DateTime lastEmailConfirmSent)
        {
            LastEmailConfirmSent = lastEmailConfirmSent;
        }

        public void AssignManager(ApplicationUser manager)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (manager.Id == Id) throw new InvalidOperationException("A user cannot be their own manager.");
            ManagerId = manager.Id;
            Manager = manager;
            manager._users.Add(this);
        }

        public void addRefreshToken(UserMetaData userMeta, string accessToken, string rToken, DateTime exprieDate)
        {
            UserRefreshTokens newRefreshToken = new UserRefreshTokens( this.Id, accessToken, rToken, exprieDate, userMeta);
            _refreshTokens.Add(newRefreshToken);
        }

        public void RevokeRefreshToken(UserRefreshTokens refreshToken)
        {
            var rToken = _refreshTokens.FirstOrDefault(r => r.Id == refreshToken.Id);
            if (rToken != null)
                rToken.Revoke();
        }
        public void RevokeAllTokens()
        {
            foreach (var token in RefreshTokens.Where(rt => !rt.IsRevoked))
            {
                token.Revoke();
            }
        }

    }
}
