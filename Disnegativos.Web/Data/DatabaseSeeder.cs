using Bogus;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.Enums;
using Disnegativos.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Disnegativos.Web.Data;

/// <summary>
/// Pobla la base de datos del servidor con datos realistas de prueba usando Bogus.
/// Solo se ejecuta en entorno DEBUG y si la BD está vacía.
/// </summary>
public static class DatabaseSeeder
{
    private const string LOCALE = "es";
    private const int SEED = 42; // Seed fijo → datos reproducibles en cada ejecución

    public static async Task SeedIfEmptyAsync(DisnegativosDbContext db)
    {
#if !DEBUG
        return; // Solo en DEBUG
#endif
        // Si ya hay datos no sembramos
        if (await db.Countries.AnyAsync()) return;

        Console.WriteLine("🌱 [Seeder] Base de datos vacía. Generando datos de prueba...");

        var faker = new Faker(LOCALE);
        Randomizer.Seed = new Random(SEED);

        // ── 1. Countries ──────────────────────────────────────────
        var countries = SeedCountries(db);
        await db.SaveChangesAsync();
        Console.WriteLine($"   ✓ {countries.Count} Countries");

        // ── 2. SportDisciplines ───────────────────────────────────
        var sports = SeedSportDisciplines(db);
        await db.SaveChangesAsync();
        Console.WriteLine($"   ✓ {sports.Count} SportDisciplines");

        // ── 3. SportCategories (por disciplina) ───────────────────
        var categories = SeedSportCategories(db, sports);
        await db.SaveChangesAsync();
        Console.WriteLine($"   ✓ {categories.Count} SportCategories");

        // ── 4. FieldPositions (por disciplina) ────────────────────
        var positions = SeedFieldPositions(db, sports);
        await db.SaveChangesAsync();
        Console.WriteLine($"   ✓ {positions.Count} FieldPositions");

        // ── IDs compartidos de tenant/customer/plan (simplificados) ─
        var tenantId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // ── 5. Organizations ──────────────────────────────────────
        var orgs = SeedOrganizations(db, faker, countries, sports, tenantId, customerId, planId, userId);
        await db.SaveChangesAsync();
        Console.WriteLine($"   ✓ {orgs.Count} Organizations");

        // ── 6. Teams ──────────────────────────────────────────────
        var teams = SeedTeams(db, faker, orgs, sports, categories, tenantId, customerId, planId, userId);
        await db.SaveChangesAsync();
        Console.WriteLine($"   ✓ {teams.Count} Teams");

        // ── 7. Players ────────────────────────────────────────────
        var players = SeedPlayers(db, faker, sports, positions, countries, orgs, teams, tenantId, customerId, planId, userId);
        await db.SaveChangesAsync();
        Console.WriteLine($"   ✓ {players.Count} Players");

        // ── 8. TeamPlayers (asignaciones) ─────────────────────────
        var teamPlayers = SeedTeamPlayers(db, faker, teams, players, positions);
        await db.SaveChangesAsync();
        Console.WriteLine($"   ✓ {teamPlayers.Count} TeamPlayers");

        // ── 9. Competitions ───────────────────────────────────────
        var competitions = SeedCompetitions(db, faker, sports, categories, customerId, planId, userId, teams);
        await db.SaveChangesAsync();
        Console.WriteLine($"   ✓ {competitions.Count} Competitions");

        // ── 10. Events ────────────────────────────────────────────
        var events = SeedEvents(db, faker, sports, categories, competitions, teams, tenantId, customerId, planId, userId);
        await db.SaveChangesAsync();
        Console.WriteLine($"   ✓ {events.Count} Events");

        // ── 11. Referees ──────────────────────────────────────────
        var referees = SeedReferees(db, faker, countries, tenantId, customerId);
        await db.SaveChangesAsync();
        Console.WriteLine($"   ✓ {referees.Count} Referees");

        Console.WriteLine("🎉 [Seeder] ¡Datos de prueba generados correctamente!");
    }

    private static List<Referee> SeedReferees(DisnegativosDbContext db, Faker f, List<Country> countries, Guid tenantId, Guid customerId)
    {
        var spain = countries.First(c => c.IsoCode == "ES");
        var categories = new[] { "Nacional A", "Nacional B", "Territorial", "Juvenil", "Regional" };

        var refereeFaker = new Faker<Referee>(LOCALE)
            .RuleFor(r => r.Id, _ => Guid.NewGuid())
            .RuleFor(r => r.TenantId, _ => tenantId)
            .RuleFor(r => r.CustomerId, _ => customerId)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.LicenseNumber, f => f.Random.Replace("???-####").ToUpper())
            .RuleFor(r => r.Category, f => f.PickRandom(categories))
            .RuleFor(r => r.Email, f => f.Internet.Email())
            .RuleFor(r => r.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(r => r.CountryId, _ => spain.Id)
            .RuleFor(r => r.IsActive, _ => true)
            .RuleFor(r => r.SyncStatus, _ => SyncStatus.Synced);

        var entities = refereeFaker.Generate(10);
        db.Referees.AddRange(entities);
        return entities;
    }

    // ══════════════════════════════════════════════════════════════
    // 1. Countries
    // ══════════════════════════════════════════════════════════════
    private static List<Country> SeedCountries(DisnegativosDbContext db)
    {
        var countries = new List<(string iso, string name, string nat, string lang, int order)>
        {
            ("ES", "España", "Español/a", "es-ES", 1),
            ("DE", "Alemania", "Alemán/a", "de-DE", 2),
            ("FR", "Francia", "Francés/a", "fr-FR", 3),
            ("IT", "Italia", "Italiano/a", "it-IT", 4),
            ("PT", "Portugal", "Portugués/a", "pt-PT", 5),
            ("GB", "Reino Unido", "Británico/a", "en-GB", 6),
            ("US", "Estados Unidos", "Estadounidense", "en-US", 7),
            ("BR", "Brasil", "Brasileño/a", "pt-BR", 8),
            ("AR", "Argentina", "Argentino/a", "es-AR", 9),
            ("MX", "México", "Mexicano/a", "es-MX", 10),
            ("NL", "Países Bajos", "Neerlandés/a", "nl-NL", 11),
            ("BE", "Bélgica", "Belga", "fr-BE", 12),
        };

        var entities = countries.Select(c => new Country
        {
            Id = Guid.NewGuid(),
            IsoCode = c.iso,
            NameKey = c.name,
            NationalityKey = c.nat,
            LanguageCode = c.lang,
            SortOrder = c.order,
            SyncStatus = SyncStatus.Synced
        }).ToList();

        db.Countries.AddRange(entities);
        return entities;
    }

    // ══════════════════════════════════════════════════════════════
    // 2. SportDisciplines
    // ══════════════════════════════════════════════════════════════
    private static List<SportDiscipline> SeedSportDisciplines(DisnegativosDbContext db)
    {
        var disciplines = new[]
        {
            new { Name = "Fútbol",       Squad = 23, Field = 11, Periods = 2, Dur = 45, GK = true,  Color = "#4CAF50" },
            new { Name = "Baloncesto",   Squad = 15, Field = 5,  Periods = 4, Dur = 12, GK = false, Color = "#FF9800" },
            new { Name = "Fútbol Sala",  Squad = 12, Field = 5,  Periods = 2, Dur = 20, GK = true,  Color = "#2196F3" },
            new { Name = "Balonmano",    Squad = 16, Field = 7,  Periods = 2, Dur = 30, GK = true,  Color = "#9C27B0" },
            new { Name = "Volleyball",   Squad = 14, Field = 6,  Periods = 5, Dur = 25, GK = false, Color = "#F44336" },
        };

        var now = DateTime.UtcNow;
        var entities = disciplines.Select(d => new SportDiscipline
        {
            Id = Guid.NewGuid(),
            NameKey = d.Name,
            SquadSize = d.Squad,
            FieldPlayerCount = d.Field,
            PeriodCount = d.Periods,
            PeriodDuration = TimeSpan.FromMinutes(d.Dur),
            OvertimeDuration = TimeSpan.FromMinutes(5),
            HasGoalkeeper = d.GK,
            IsActive = true,
            ActivationDate = now,
            FieldColor = d.Color,
            SyncStatus = SyncStatus.Synced
        }).ToList();

        db.SportDisciplines.AddRange(entities);
        return entities;
    }

    // ══════════════════════════════════════════════════════════════
    // 3. SportCategories
    // ══════════════════════════════════════════════════════════════
    private static List<SportCategory> SeedSportCategories(DisnegativosDbContext db, List<SportDiscipline> sports)
    {
        var categoryNames = new[] { "Infantil A", "Infantil B", "Cadete", "Juvenil", "Senior", "Veteranos", "Femenino" };
        var categories = new List<SportCategory>();

        foreach (var sport in sports)
        {
            // 4-5 categorías por deporte
            foreach (var catName in categoryNames.Take(new Random(SEED).Next(4, 6)))
            {
                categories.Add(new SportCategory
                {
                    Id = Guid.NewGuid(),
                    SportDisciplineId = sport.Id,
                    Name = catName,
                    SyncStatus = SyncStatus.Synced
                });
            }
        }

        db.SportCategories.AddRange(categories);
        return categories;
    }

    // ══════════════════════════════════════════════════════════════
    // 4. FieldPositions
    // ══════════════════════════════════════════════════════════════
    private static List<FieldPosition> SeedFieldPositions(DisnegativosDbContext db, List<SportDiscipline> sports)
    {
        var positionsBySport = new Dictionary<string, string[]>
        {
            ["Fútbol"]      = ["Portero", "Defensa Central", "Lateral Derecho", "Lateral Izquierdo", "Mediocentro", "Mediocampista", "Extremo Derecho", "Extremo Izquierdo", "Delantero Centro", "Segundo Delantero"],
            ["Baloncesto"]  = ["Base", "Escolta", "Alero", "Ala-Pívot", "Pívot"],
            ["Fútbol Sala"] = ["Portero", "Cierre", "Ala Derecha", "Ala Izquierda", "Pívot"],
            ["Balonmano"]   = ["Portero", "Lateral Derecho", "Lateral Izquierdo", "Central", "Extremo Derecho", "Extremo Izquierdo", "Pivote"],
            ["Volleyball"]  = ["Colocador", "Opuesto", "Central", "Receptor", "Líbero"],
        };

        var positions = new List<FieldPosition>();
        foreach (var sport in sports)
        {
            if (!positionsBySport.TryGetValue(sport.NameKey, out var names)) continue;
            foreach (var name in names)
            {
                positions.Add(new FieldPosition
                {
                    Id = Guid.NewGuid(),
                    SportDisciplineId = sport.Id,
                    Name = name,
                    SyncStatus = SyncStatus.Synced
                });
            }
        }

        db.FieldPositions.AddRange(positions);
        return positions;
    }

    // ══════════════════════════════════════════════════════════════
    // 5. Organizations
    // ══════════════════════════════════════════════════════════════
    private static List<Organization> SeedOrganizations(
        DisnegativosDbContext db, Faker f,
        List<Country> countries, List<SportDiscipline> sports,
        Guid tenantId, Guid customerId, Guid planId, Guid userId)
    {
        var sport = sports.First(); // Fútbol
        var spain = countries.First(c => c.IsoCode == "ES");

        var orgFaker = new Faker<Organization>(LOCALE)
            .RuleFor(o => o.Id, _ => Guid.NewGuid())
            .RuleFor(o => o.TenantId, _ => tenantId)
            .RuleFor(o => o.CustomerId, _ => customerId)
            .RuleFor(o => o.ServicePlanId, _ => planId)
            .RuleFor(o => o.UserId, _ => userId)
            .RuleFor(o => o.SportDisciplineId, _ => sport.Id)
            .RuleFor(o => o.Name, f => $"Club {f.Address.City()}")
            .RuleFor(o => o.Address, f => f.Address.StreetAddress())
            .RuleFor(o => o.City, f => f.Address.City())
            .RuleFor(o => o.PostalCode, f => f.Address.ZipCode())
            .RuleFor(o => o.Province, f => f.Address.State())
            .RuleFor(o => o.CountryId, _ => spain.Id)
            .RuleFor(o => o.Email, f => f.Internet.Email())
            .RuleFor(o => o.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(o => o.Website, f => f.Internet.Url())
            .RuleFor(o => o.IsActive, _ => true)
            .RuleFor(o => o.ActivationDate, f => f.Date.Past(2))
            .RuleFor(o => o.SyncStatus, _ => SyncStatus.Synced);

        var orgs = orgFaker.Generate(6);
        db.Organizations.AddRange(orgs);
        return orgs;
    }

    // ══════════════════════════════════════════════════════════════
    // 6. Teams
    // ══════════════════════════════════════════════════════════════
    private static List<Team> SeedTeams(
        DisnegativosDbContext db, Faker f,
        List<Organization> orgs, List<SportDiscipline> sports,
        List<SportCategory> categories,
        Guid tenantId, Guid customerId, Guid planId, Guid userId)
    {
        var sport = sports.First();
        var sportCategories = categories.Where(c => c.SportDisciplineId == sport.Id).ToList();
        var teams = new List<Team>();

        // 2 equipos por organización
        foreach (var org in orgs)
        {
            var cat = new Faker().PickRandom(sportCategories);
            var cityParts = org.City?.Split(' ') ?? ["Team"];
            var abbrev = string.Concat(cityParts.Take(3).Select(p => p[0])).ToUpper();

            teams.Add(new Team
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerId = customerId,
                ServicePlanId = planId,
                SportDisciplineId = sport.Id,
                SportCategoryId = cat.Id,
                UserId = userId,
                Name = $"{org.Name} {cat.Name}",
                Alias = abbrev,
                IsActive = true,
                ActivationDate = DateTime.UtcNow.AddYears(-1),
                SyncStatus = SyncStatus.Synced
            });

            var cat2 = new Faker().PickRandom(sportCategories.Where(c => c.Id != cat.Id).ToList());
            teams.Add(new Team
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerId = customerId,
                ServicePlanId = planId,
                SportDisciplineId = sport.Id,
                SportCategoryId = cat2.Id,
                UserId = userId,
                Name = $"{org.Name} {cat2.Name}",
                Alias = abbrev + "2",
                IsActive = true,
                ActivationDate = DateTime.UtcNow.AddYears(-1),
                SyncStatus = SyncStatus.Synced
            });
        }

        db.Teams.AddRange(teams);
        return teams;
    }

    // ══════════════════════════════════════════════════════════════
    // 7. Players
    // ══════════════════════════════════════════════════════════════
    private static List<Player> SeedPlayers(
        DisnegativosDbContext db, Faker f,
        List<SportDiscipline> sports, List<FieldPosition> positions,
        List<Country> countries, List<Organization> orgs,
        List<Team> teams,
        Guid tenantId, Guid customerId, Guid planId, Guid userId)
    {
        var sport = sports.First();
        var sportPositions = positions.Where(p => p.SportDisciplineId == sport.Id).ToList();
        var playerFakes = new Faker<Player>(LOCALE)
            .RuleFor(p => p.Id, _ => Guid.NewGuid())
            .RuleFor(p => p.TenantId, _ => tenantId)
            .RuleFor(p => p.CustomerId, _ => customerId)
            .RuleFor(p => p.ServicePlanId, _ => planId)
            .RuleFor(p => p.SportDisciplineId, _ => sport.Id)
            .RuleFor(p => p.UserId, _ => userId)
            .RuleFor(p => p.OrganizationId, f => f.PickRandom(orgs).Id)
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName())
            .RuleFor(p => p.Nickname, f => f.Internet.UserName())
            .RuleFor(p => p.DateOfBirth, f => f.Date.Past(25, DateTime.Now.AddYears(-14)))
            .RuleFor(p => p.Gender, _ => 0) // 0=Masculino
            .RuleFor(p => p.CountryId, f => f.PickRandom(countries).Id)
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(p => p.JerseyNumber, f => f.Random.Number(1, 99).ToString())
            .RuleFor(p => p.Height, f => $"{f.Random.Number(165, 200)} cm")
            .RuleFor(p => p.Weight, f => $"{f.Random.Number(60, 100)} kg")
            .RuleFor(p => p.PreferredFoot, f => f.PickRandom("Derecho", "Izquierdo", "Ambidiestro"))
            .RuleFor(p => p.FieldPositionId, f => f.PickRandom(sportPositions).Id)
            .RuleFor(p => p.IsActive, _ => true)
            .RuleFor(p => p.ActivationDate, f => f.Date.Past(3))
            .RuleFor(p => p.SyncStatus, _ => SyncStatus.Synced);

        // 20 jugadores por equipo (primer equipo), 15 los demás
        var players = playerFakes.Generate(orgs.Count * 22);
        db.Players.AddRange(players);
        return players;
    }

    // ══════════════════════════════════════════════════════════════
    // 8. TeamPlayers
    // ══════════════════════════════════════════════════════════════
    private static List<TeamPlayer> SeedTeamPlayers(
        DisnegativosDbContext db, Faker f,
        List<Team> teams, List<Player> players,
        List<FieldPosition> positions)
    {
        var teamPlayers = new List<TeamPlayer>();
        var shuffled = players.OrderBy(_ => f.Random.Int()).ToList();
        var chunkSize = Math.Max(1, shuffled.Count / teams.Count);

        for (int i = 0; i < teams.Count; i++)
        {
            var teamChunk = shuffled.Skip(i * chunkSize).Take(chunkSize).ToList();
            var sportPositions = positions.Where(p => p.SportDisciplineId == teams[i].SportDisciplineId).ToList();

            var jerseyNumbers = Enumerable.Range(1, 99).OrderBy(_ => f.Random.Int()).ToList();
            int jerseyIdx = 0;

            foreach (var player in teamChunk)
            {
                teamPlayers.Add(new TeamPlayer
                {
                    Id = Guid.NewGuid(),
                    TeamId = teams[i].Id,
                    PlayerId = player.Id,
                    FieldPositionId = sportPositions.Count > 0 ? f.PickRandom(sportPositions).Id : null,
                    JerseyNumber = jerseyIdx < jerseyNumbers.Count ? jerseyNumbers[jerseyIdx++].ToString() : null,
                    IsActive = true,
                    ActivationDate = DateTime.UtcNow.AddMonths(-6),
                    StartDate = DateTime.UtcNow.AddMonths(-6),
                    SyncStatus = SyncStatus.Synced
                });
            }
        }

        db.TeamPlayers.AddRange(teamPlayers);
        return teamPlayers;
    }

    // ══════════════════════════════════════════════════════════════
    // 9. Competitions
    // ══════════════════════════════════════════════════════════════
    private static List<Competition> SeedCompetitions(
        DisnegativosDbContext db, Faker f,
        List<SportDiscipline> sports, List<SportCategory> categories,
        Guid customerId, Guid planId, Guid userId,
        List<Team> teams)
    {
        var sport = sports.First();
        var sportCategories = categories.Where(c => c.SportDisciplineId == sport.Id).ToList();
        var competitionNames = new[]
        {
            "Liga Regional", "Copa Federación", "Torneo de Verano",
            "Supercopa", "Copa del Rey Local", "Torneo Navidad",
            "Trofeo Apertura", "Liga Municipal"
        };
        var colors = new[] { "#E53935", "#8E24AA", "#1E88E5", "#43A047", "#F4511E", "#F6BF26", "#039BE5", "#0B8043" };

        var season = DateTime.Now.Year;
        var competitions = competitionNames.Select((name, idx) => new Competition
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ServicePlanId = planId,
            SportDisciplineId = sport.Id,
            SportCategoryId = f.PickRandom(sportCategories).Id,
            UserId = userId,
            TeamId = teams.Count > 0 ? f.PickRandom(teams).Id : null,
            Title = $"{name} {season}-{season + 1}",
            StartDate = new DateTime(season, 9, 1),
            EndDate = new DateTime(season + 1, 6, 30),
            Color = colors[idx % colors.Length],
            IsActive = true,
            ShowInCalendar = true,
            SyncStatus = SyncStatus.Synced
        }).ToList();

        db.Competitions.AddRange(competitions);
        return competitions;
    }

    // ══════════════════════════════════════════════════════════════
    // 10. Events
    // ══════════════════════════════════════════════════════════════
    private static List<Event> SeedEvents(
        DisnegativosDbContext db, Faker f,
        List<SportDiscipline> sports, List<SportCategory> categories,
        List<Competition> competitions, List<Team> teams,
        Guid tenantId, Guid customerId, Guid planId, Guid userId)
    {
        var sport = sports.First();
        var sportCategories = categories.Where(c => c.SportDisciplineId == sport.Id).ToList();
        var events = new List<Event>();

        // 10 eventos por competición
        foreach (var competition in competitions)
        {
            var compTeams = teams.Where(t => t.SportDisciplineId == sport.Id).ToList();
            if (compTeams.Count < 2) continue;

            for (int i = 0; i < 10; i++)
            {
                var homeTeam = f.PickRandom(compTeams);
                var awayTeam = f.PickRandom(compTeams.Where(t => t.Id != homeTeam.Id).ToList());
                if (awayTeam == null) continue;

                var eventDate = f.Date.Between(
                    competition.StartDate ?? DateTime.Now.AddMonths(-3),
                    competition.EndDate ?? DateTime.Now.AddMonths(3));

                var isPast = eventDate < DateTime.Now;
                var result = isPast ? $"{f.Random.Number(0, 5)}-{f.Random.Number(0, 5)}" : null;

                events.Add(new Event
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    CustomerId = customerId,
                    ServicePlanId = planId,
                    SportDisciplineId = sport.Id,
                    SportCategoryId = competition.SportCategoryId,
                    CompetitionId = competition.Id,
                    UserId = userId,
                    HomeTeamId = homeTeam.Id,
                    AwayTeamId = awayTeam.Id,
                    Name = $"{homeTeam.Name} vs {awayTeam.Name}",
                    ShortName = $"{homeTeam.Alias} - {awayTeam.Alias}",
                    IsHomeGame = f.Random.Bool(),
                    Result = result,
                    StartDate = eventDate.Date,
                    StartTime = TimeSpan.FromHours(f.Random.Number(10, 20)),
                    MeetingTime = TimeSpan.FromHours(f.Random.Number(9, 19)),
                    Matchday = $"J{i + 1}",
                    Phase = "Fase Regular",
                    IsActive = true,
                    ActivationDate = DateTime.UtcNow,
                    SyncStatus = SyncStatus.Synced
                });
            }
        }

        db.Events.AddRange(events);
        return events;
    }
}
