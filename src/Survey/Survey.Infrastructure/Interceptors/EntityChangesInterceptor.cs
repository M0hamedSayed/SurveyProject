using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Survey.Domain.Interfaces.Models;

namespace Survey.Infrastructure.Interceptors
{
    public sealed class EntityChangesInterceptor : Microsoft.EntityFrameworkCore.Diagnostics.SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext? context)
        {
            if (context is null) return;

            // handle updated entity
            IEnumerable<EntityEntry<IEntity>> updatedEntries = context
                    .ChangeTracker
                    .Entries<IEntity>()
                    .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (EntityEntry<IEntity> entity in updatedEntries)
            {
                var now = DateTime.UtcNow; // current datetime

                if (entity.State == EntityState.Added)
                {
                    ((IEntity)entity.Entity).CreatedAt = now;
                }
                ((IEntity)entity.Entity).UpdatedAt = now;
            }

            // handle soft deleted entity
            IEnumerable<EntityEntry<IEntitySoftDeletable>> entries = context
                    .ChangeTracker
                    .Entries<IEntitySoftDeletable>()
                    .Where(e => e.State == EntityState.Deleted);

            foreach (EntityEntry<IEntitySoftDeletable> softDeletable in entries)
            {
                softDeletable.State = EntityState.Modified;
                softDeletable.Entity.IsDeleted = true;
                softDeletable.Entity.DeletedAt = DateTime.UtcNow;
            }

        }

    }
}
