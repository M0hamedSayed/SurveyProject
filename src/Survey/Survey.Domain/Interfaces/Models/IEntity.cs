namespace Survey.Domain.Interfaces.Models
{
    
    public interface IEntity<T> : IEntity
    { 
        public T Id { get; set; }
    }
    
    public interface IEntity
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public interface IEntitySoftDeletable<T> : IEntity<T>, IEntitySoftDeletable
    {
    }
    public interface IEntitySoftDeletable : IEntity
    {
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
