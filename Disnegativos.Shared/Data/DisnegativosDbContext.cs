using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data;

public class DisnegativosDbContext : DbContext
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<SubCustomer> SubCustomers => Set<SubCustomer>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<ServicePlan> ServicePlans => Set<ServicePlan>();
    public DbSet<User> Users => Set<User>();
    
    // Core Domain
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Competition> Competitions => Set<Competition>();
    public DbSet<Event> Events => Set<Event>();

    // Tools
    public DbSet<SyncLog> SyncLogs => Set<SyncLog>();

    public DisnegativosDbContext(DbContextOptions<DisnegativosDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DisnegativosDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Id = entry.Entity.Id == Guid.Empty ? Guid.NewGuid() : entry.Entity.Id;
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    // Soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsArchived = true;
                    entry.Entity.ArchivedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(ct);
    }
}
