using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaFashion.API.Entities;

namespace NovaFashion.API.Persistence.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.AltText)
               .IsRequired(false)
               .HasMaxLength(100);

            builder.Property(x => x.IsPrimary)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany(x => x.ProductImages)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
