using System.ComponentModel.DataAnnotations.Schema;
using Survey.Domain.Interfaces.Models;

namespace Survey.Domain.Abstractions
{
    public abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public abstract class EntitySoftDeletable<T> : Entity<T>,IEntitySoftDeletable<T>
    {
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }
    }
}
