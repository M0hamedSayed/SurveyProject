using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notify.API.Models
{
    [Table("notifications", Schema = "Notification")]
    [Index(nameof(UserEmail), IsUnique = false)]
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string UserEmail { get; set; }
        public required string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty ;
        public bool IsRead { get; set; } = false;
    }
}
