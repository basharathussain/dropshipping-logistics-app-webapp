using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

internal sealed class LoadAuditLogEntityConfiguration : IEntityTypeConfiguration<LoadAuditLog>
{
    public void Configure(EntityTypeBuilder<LoadAuditLog> builder)
    {
        builder.ToTable("load_audit_logs");

        builder.HasIndex(x => x.LoadId);

        builder.Property(x => x.Action).HasMaxLength(50);
        builder.Property(x => x.Changes).HasColumnType("text");

        builder.HasOne<Load>()
            .WithMany()
            .HasForeignKey(x => x.LoadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
