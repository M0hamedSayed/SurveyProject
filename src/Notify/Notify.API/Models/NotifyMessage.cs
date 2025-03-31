using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Notify.API.Models
{
    [Table("notify_messages", Schema = "Notification")]
    [Index(nameof(SentAt), IsUnique = false)]
    public class NotifyMessage
    {
        [Key]
        public Guid Id { get; set; }
        public NotificationTypes NotificationType { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

    }
}
