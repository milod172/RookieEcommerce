using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaFashion.API.Entities;

namespace NovaFashion.API.Infrastructure.Persistence.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.Property(x => x.Size)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.VariantSku)
               .HasMaxLength(50);

            builder.Property(x => x.UnitPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Product)
                .WithMany(x => x.ProductVariants)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(x => x.OrderItems)
            //    .WithOne(x => x.ProductVariant)
            //    .HasForeignKey(x => x.ProductVariantId);

            //builder.HasMany(x => x.CartItems)
            //   .WithOne(x => x.ProductVariant)
            //   .HasForeignKey(x => x.ProductVariantId);

        }
    }
}
