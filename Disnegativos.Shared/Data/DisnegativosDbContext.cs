using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data;

public class DisnegativosDbContext : DbContext
{
    // Organization & Tenancy
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<SubCustomer> SubCustomers => Set<SubCustomer>();
    public DbSet<ServicePlan> ServicePlans => Set<ServicePlan>();
    public DbSet<User> Users => Set<User>();
    
    // Reference & Catalog
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<SportDiscipline> SportDisciplines => Set<SportDiscipline>();
    public DbSet<SportCategory> SportCategories => Set<SportCategory>();
    public DbSet<FieldPosition> FieldPositions => Set<FieldPosition>();

    // Entities & Teams
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<TeamPlayer> TeamPlayers => Set<TeamPlayer>();

    // Competitions & Events
    public DbSet<Competition> Competitions => Set<Competition>();
    public DbSet<Event> Events => Set<Event>();

    // Analysis Templates
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<Panel> Panels => Set<Panel>();
    public DbSet<Block> Blocks => Set<Block>();
    public DbSet<Button> Buttons => Set<Button>();
    public DbSet<Concept> Concepts => Set<Concept>();

    // Analysis & Actions
    public DbSet<Analysis> Analyses => Set<Analysis>();
    public DbSet<AnalysisMedia> AnalysisMedias => Set<AnalysisMedia>();
    public DbSet<GamePeriod> GamePeriods => Set<GamePeriod>();
    public DbSet<MatchAction> MatchActions => Set<MatchAction>();
    public DbSet<ActionPlayer> ActionPlayers => Set<ActionPlayer>();
    public DbSet<ActionConcept> ActionConcepts => Set<ActionConcept>();

    // Reports & Slides
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Slide> Slides => Set<Slide>();

    // People (Staff)
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<PersonRole> PersonRoles => Set<PersonRole>();
    public DbSet<Referee> Referees => Set<Referee>();

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
