using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Disnegativos.Shared.Data.Entities;

namespace Disnegativos.Shared.Data.Configurations;

public class MatchActionConfiguration : BaseEntityConfiguration<MatchAction>
{
    public override void Configure(EntityTypeBuilder<MatchAction> builder)
    {
        base.Configure(builder);

        // Configuración de relaciones recursivas para evitar ambigüedad en 1:1
        builder.HasOne(a => a.ParentAction)
               .WithMany() // O si quieres navegación inversa .WithMany(a => a.ChildActions)
               .HasForeignKey(a => a.ParentActionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.RelatedAction)
               .WithMany()
               .HasForeignKey(a => a.RelatedActionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
