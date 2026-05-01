using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NovaFashion.API.Shared.Abstractions;
using NovaFashion.API.Shared.Extensions;

namespace NovaFashion.API.Infrastructure.Persistence.Interceptors
{
    public class AuditInterceptor() : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
        {
            HandleSoftDelete(eventData.Context);
            UpdateAuditFields(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void HandleSoftDelete(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries<IHasAudit>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                }
            }
        }

        private void UpdateAuditFields(DbContext? context)
        {
            if (context == null) return;

            var now = DateTime.UtcNow;
            //var userId = _currentUserService.UserId ?? "System";   

            foreach (var entry in context.ChangeTracker.Entries<IHasAudit>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreated();
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetModified();
                    
                    var isDeletedProp = entry.Property(nameof(IHasAudit.IsDeleted));

                    if (isDeletedProp.IsModified)
                    {
                        if (entry.Entity.IsDeleted)
                        {
                            entry.Entity.SetDeleted();
                        }
                        else
                        {
                            entry.Entity.DeletedAt = null;
                        }
                    }
                }
            }
        }
    }
}
