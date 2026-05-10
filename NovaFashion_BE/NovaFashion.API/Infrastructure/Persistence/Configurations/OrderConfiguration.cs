using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaFashion.API.Entities;

namespace NovaFashion.API.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Orders> builder)
        {
            builder.Property(x => x.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PhoneNumber)
              .IsRequired()
              .HasMaxLength(20);

            builder.Property(x => x.ShippingAddress)
                .IsRequired()
                .HasMaxLength(225);

            builder.Property(x => x.PaymentMethod)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(x => x.PaymentStatus)
                 .IsRequired()
                 .HasConversion<string>();

            builder.Property(x => x.OrderStatus)
                .IsRequired()
                .HasConversion<string>();


            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
