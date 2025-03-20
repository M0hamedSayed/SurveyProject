using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Survey.Domain.Abstractions;
using Survey.Domain.ValueObjects.Identity;

namespace Survey.Domain.Models.Identity
{
    [Table("user_refresh_tokens")]
    [Comment("Handle refresh Tokens for users")]
    [Index(nameof(UserId), IsUnique = false)]
    public class UserRefreshTokens : Entity<RefreshTokenId>
    {
        [Column("user_id")]
        public Guid UserId { get; private set; }
        [Column("access_token")]
        public string AccessToken { get; private set; }
        [Column("refresh_token")]
        public string RefreshToken { get; private set; }
        [Column("is_used")]
        public bool IsUsed { get; private set; }
        [Column("is_revoked")]
        public bool IsRevoked { get; private set; }
        [Column("added_time")]
        public DateTime AddedTime { get; private set; }
        [Column("expiry_date")]
        public DateTime ExpiryDate { get; private set; }

        // user metadata
        public UserMetaData UserMetaData { get; private set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; private set; }

        private UserRefreshTokens() { }
        public UserRefreshTokens (Guid userId, string accessToken, string refreshToken, DateTime expiryDate, UserMetaData userMetaData)
        {
            Id = RefreshTokenId.Of(Guid.NewGuid());
            UserId = userId;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            IsUsed = false;
            IsRevoked = false;
            AddedTime = DateTime.UtcNow;
            ExpiryDate = expiryDate;
            UserMetaData = userMetaData;
        }

        public void Revoke()
        {
            IsRevoked = true;
        }

        public void AssignNewAccessToken( string accessToken)
        {
            AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        }
    }
}
