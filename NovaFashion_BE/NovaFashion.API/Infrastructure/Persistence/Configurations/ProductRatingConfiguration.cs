using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaFashion.API.Entities;

namespace NovaFashion.API.Infrastructure.Persistence.Configurations
{
    public class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
    {
        public void Configure(EntityTypeBuilder<ProductRating> builder)
        {
            builder.Property(x => x.Rate)
                .IsRequired();
            
            builder.Property(x => x.Comment)
               .HasMaxLength(1000)
               .IsRequired(false);

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
                .WithMany(p => p.ProductRatings)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pr => pr.Orders)
                 .WithOne(o => o.ProductRating)
                 .HasForeignKey<ProductRating>(pr => pr.OrderId)
                 .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
