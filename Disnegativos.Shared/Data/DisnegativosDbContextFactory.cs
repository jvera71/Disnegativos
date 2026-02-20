using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Disnegativos.Shared.Data;

public class DisnegativosDbContextFactory : IDesignTimeDbContextFactory<DisnegativosDbContext>
{
    public DisnegativosDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DisnegativosDbContext>();
        optionsBuilder.UseSqlite("Data Source=disnegativos_design.db");

        return new DisnegativosDbContext(optionsBuilder.Options);
    }
}
