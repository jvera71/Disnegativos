using Dotmim.Sync;

namespace Disnegativos.Shared.Data;

public static class SyncSetupProvider
{
    public static SyncSetup GetSyncSetup()
    {
        var setup = new SyncSetup(new string[]
        {
            "Tenants",
            "Customers",
            "SubCustomers",
            "ServicePlans",
            "Users",
            "Countries",
            "SportDisciplines",
            "SportCategories",
            "FieldPositions",
            "Organizations",
            "Teams",
            "Players",
            "TeamPlayers",
            "Competitions",
            "Events",
            "Templates",
            "Panels",
            "Blocks",
            "Buttons",
            "Concepts",
            "Analyses",
            "AnalysisMedias",
            "GamePeriods",
            "MatchActions",
            "ActionPlayers",
            "ActionConcepts",
            "Reports",
            "Slides",
            "Persons",
            "PersonRoles",
            "Referees"
        });

        return setup;
    }
}
