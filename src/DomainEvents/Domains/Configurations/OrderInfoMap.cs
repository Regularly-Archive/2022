using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainEvents.Domains.Configurations
{
    public class OrderInfoMap : IEntityTypeConfiguration<OrderInfo>
    {
        public void Configure(EntityTypeBuilder<OrderInfo> builder)
        {
            builder.ToTable("OrderInfo");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Remark).HasColumnName("Remark");
            builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
            builder.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        }
    }
}
