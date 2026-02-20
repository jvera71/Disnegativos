using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Disnegativos.Shared.Data.Entities;

namespace Disnegativos.Shared.Data.Configurations;

public class TenantConfiguration : BaseEntityConfiguration<Tenant>
{
    public override void Configure(EntityTypeBuilder<Tenant> builder)
    {
        base.Configure(builder);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
    }
}
