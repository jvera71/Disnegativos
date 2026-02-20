using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Disnegativos.Shared.Data.Entities;

namespace Disnegativos.Shared.Data.Configurations;

public class EventConfiguration : BaseEntityConfiguration<Event>
{
    public override void Configure(EntityTypeBuilder<Event> builder)
    {
        base.Configure(builder);

        builder.HasOne(e => e.HomeTeam)
               .WithMany()
               .HasForeignKey(e => e.HomeTeamId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AwayTeam)
               .WithMany()
               .HasForeignKey(e => e.AwayTeamId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
