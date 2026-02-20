using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Disnegativos.Shared.Data.Entities;

namespace Disnegativos.Shared.Data.Configurations;

public class AnalysisConfiguration : BaseEntityConfiguration<Analysis>
{
    public override void Configure(EntityTypeBuilder<Analysis> builder)
    {
        base.Configure(builder);

        builder.HasOne(a => a.AnalyzedTeam)
               .WithMany()
               .HasForeignKey(a => a.AnalyzedTeamId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.OpponentTeam)
               .WithMany()
               .HasForeignKey(a => a.OpponentTeamId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
