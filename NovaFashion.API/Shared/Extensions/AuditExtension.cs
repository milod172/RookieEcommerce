using NovaFashion.API.Shared.Abstractions;

namespace NovaFashion.API.Shared.Extensions
{
    public static class AuditExtensions
    {
        public static void SetCreated(this IHasAudit entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
        }

        public static void SetModified(this IHasAudit entity)
        {
            entity.ModifiedTime = DateTime.UtcNow;
        }
    }
}
