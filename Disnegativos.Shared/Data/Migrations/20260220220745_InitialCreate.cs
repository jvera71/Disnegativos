using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disnegativos.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsoCode = table.Column<string>(type: "TEXT", nullable: false),
                    NameKey = table.Column<string>(type: "TEXT", nullable: false),
                    NationalityKey = table.Column<string>(type: "TEXT", nullable: false),
                    LanguageCode = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServicePlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SecondsBeforeClip = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondsAfterClip = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxTemplates = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxPanels = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxBlocks = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxButtons = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxAnalyses = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowFieldView = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowOrgChart = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeviceOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    BucketName = table.Column<string>(type: "TEXT", nullable: true),
                    AllowMediaUtils = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowDrawings = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowConcatVideo = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowMultiVideo = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SportDisciplines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NameKey = table.Column<string>(type: "TEXT", nullable: false),
                    ImageFileId = table.Column<string>(type: "TEXT", nullable: true),
                    SquadSize = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldPlayerCount = table.Column<int>(type: "INTEGER", nullable: false),
                    PeriodCount = table.Column<int>(type: "INTEGER", nullable: false),
                    OvertimeCount = table.Column<int>(type: "INTEGER", nullable: true),
                    PeriodDuration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    OvertimeDuration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    HasGoalkeeper = table.Column<bool>(type: "INTEGER", nullable: false),
                    AutoOvertime = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    FieldColor = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportDisciplines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SyncLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TableName = table.Column<string>(type: "TEXT", nullable: false),
                    RecordId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OperationType = table.Column<int>(type: "INTEGER", nullable: false),
                    ChangedFields = table.Column<string>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsSynced = table.Column<bool>(type: "INTEGER", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ConflictData = table.Column<string>(type: "TEXT", nullable: true),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServicePlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportDisciplineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    ExtendedAddress = table.Column<string>(type: "TEXT", nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Province = table.Column<string>(type: "TEXT", nullable: true),
                    CountryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Website = table.Column<string>(type: "TEXT", nullable: true),
                    LogoFileId = table.Column<string>(type: "TEXT", nullable: true),
                    IsOrgChart = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Organizations_SportDisciplines_SportDisciplineId",
                        column: x => x.SportDisciplineId,
                        principalTable: "SportDisciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SportDisciplineId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonRoles_SportDisciplines_SportDisciplineId",
                        column: x => x.SportDisciplineId,
                        principalTable: "SportDisciplines",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServicePlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportDisciplineId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Author = table.Column<string>(type: "TEXT", nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ToggleableBlocks = table.Column<bool>(type: "INTEGER", nullable: false),
                    CollapsibleBlocks = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Templates_SportDisciplines_SportDisciplineId",
                        column: x => x.SportDisciplineId,
                        principalTable: "SportDisciplines",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Alias = table.Column<string>(type: "TEXT", nullable: true),
                    FiscalAddress = table.Column<string>(type: "TEXT", nullable: true),
                    ExtendedAddress = table.Column<string>(type: "TEXT", nullable: true),
                    CountryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Province = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: true),
                    TaxId = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Website = table.Column<string>(type: "TEXT", nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    LogoFileId = table.Column<string>(type: "TEXT", nullable: true),
                    IsOnline = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    StatusDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Customers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Panels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    BackgroundColor = table.Column<string>(type: "TEXT", nullable: true),
                    ForegroundColor = table.Column<string>(type: "TEXT", nullable: true),
                    FontSize = table.Column<int>(type: "INTEGER", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SecondsBeforeClip = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondsAfterClip = table.Column<int>(type: "INTEGER", nullable: false),
                    LevelCount = table.Column<int>(type: "INTEGER", nullable: true),
                    Padding = table.Column<int>(type: "INTEGER", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Panels_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldPositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportDisciplineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldPositions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FieldPositions_SportDisciplines_SportDisciplineId",
                        column: x => x.SportDisciplineId,
                        principalTable: "SportDisciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServicePlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportDisciplineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    CountryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    ImageFileId = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Persons_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Persons_PersonRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "PersonRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Persons_SportDisciplines_SportDisciplineId",
                        column: x => x.SportDisciplineId,
                        principalTable: "SportDisciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SportCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportDisciplineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SportCategories_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SportCategories_SportDisciplines_SportDisciplineId",
                        column: x => x.SportDisciplineId,
                        principalTable: "SportDisciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    ExtendedAddress = table.Column<string>(type: "TEXT", nullable: true),
                    CountryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Province = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: true),
                    TaxId = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Website = table.Column<string>(type: "TEXT", nullable: true),
                    LogoFileId = table.Column<string>(type: "TEXT", nullable: true),
                    IsOnline = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    StatusDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCustomers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubCustomers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PanelId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    BackgroundColor = table.Column<string>(type: "TEXT", nullable: true),
                    ForegroundColor = table.Column<string>(type: "TEXT", nullable: true),
                    FontSize = table.Column<int>(type: "INTEGER", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHeaderVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowCounter = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFixed = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsOptionMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    SecondsBeforeClip = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondsAfterClip = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowStatistics = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blocks_Panels_PanelId",
                        column: x => x.PanelId,
                        principalTable: "Panels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Concepts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PanelId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryType = table.Column<int>(type: "INTEGER", nullable: true),
                    IsToggle = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresTeam = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresPlayer = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresZone = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowStatistics = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsOptionMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    OptionGroupName = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Concepts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Concepts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Concepts_Panels_PanelId",
                        column: x => x.PanelId,
                        principalTable: "Panels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Competitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServicePlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportDisciplineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportCategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Color = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPrivate = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowInCalendar = table.Column<bool>(type: "INTEGER", nullable: false),
                    ImageFileId = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Competitions_SportCategories_SportCategoryId",
                        column: x => x.SportCategoryId,
                        principalTable: "SportCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServicePlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportDisciplineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportCategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Alias = table.Column<string>(type: "TEXT", nullable: true),
                    ImageFileId = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Teams_SportCategories_SportCategoryId",
                        column: x => x.SportCategoryId,
                        principalTable: "SportCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubCustomerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ServicePlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    CountryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Gender = table.Column<int>(type: "INTEGER", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    NotificationEmail = table.Column<string>(type: "TEXT", nullable: true),
                    UserType = table.Column<string>(type: "TEXT", nullable: true),
                    PreferredLocale = table.Column<string>(type: "TEXT", nullable: false),
                    TimeZoneId = table.Column<string>(type: "TEXT", nullable: false),
                    AvatarFileId = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsBlocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    BlockedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_ServicePlans_ServicePlanId",
                        column: x => x.ServicePlanId,
                        principalTable: "ServicePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_SubCustomers_SubCustomerId",
                        column: x => x.SubCustomerId,
                        principalTable: "SubCustomers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Buttons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlockId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConceptId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ParentButtonId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ButtonCategoryType = table.Column<int>(type: "INTEGER", nullable: false),
                    BackgroundColor = table.Column<string>(type: "TEXT", nullable: true),
                    ForegroundColor = table.Column<string>(type: "TEXT", nullable: true),
                    SecondsBeforeClip = table.Column<double>(type: "REAL", nullable: false),
                    SecondsAfterClip = table.Column<double>(type: "REAL", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAttribute = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsInOut = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsToggle = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasChildren = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresTeam = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresPlayer = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresZone = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowStatistics = table.Column<bool>(type: "INTEGER", nullable: false),
                    Shortcut = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    BackgroundImage = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buttons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buttons_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Buttons_Buttons_ParentButtonId",
                        column: x => x.ParentButtonId,
                        principalTable: "Buttons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Buttons_Concepts_ConceptId",
                        column: x => x.ConceptId,
                        principalTable: "Concepts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServicePlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportDisciplineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportCategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompetitionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    HomeTeamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AwayTeamId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ShortName = table.Column<string>(type: "TEXT", nullable: true),
                    IsHomeGame = table.Column<bool>(type: "INTEGER", nullable: false),
                    Result = table.Column<string>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    MeetingTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    Matchday = table.Column<string>(type: "TEXT", nullable: true),
                    Phase = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    JsonData = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_Teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServicePlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SportDisciplineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DefaultTeamId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FieldPositionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PlayerType = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    Nickname = table.Column<string>(type: "TEXT", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    CountryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SecondCountryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    JerseyNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Height = table.Column<string>(type: "TEXT", nullable: true),
                    Weight = table.Column<string>(type: "TEXT", nullable: true),
                    PreferredFoot = table.Column<string>(type: "TEXT", nullable: true),
                    ImageFileId = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Players_Countries_SecondCountryId",
                        column: x => x.SecondCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Players_FieldPositions_FieldPositionId",
                        column: x => x.FieldPositionId,
                        principalTable: "FieldPositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Players_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Players_SportDisciplines_SportDisciplineId",
                        column: x => x.SportDisciplineId,
                        principalTable: "SportDisciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Players_Teams_DefaultTeamId",
                        column: x => x.DefaultTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AnalyzedTeamId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OpponentTeamId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TotalDuration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    HasVideo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMultiVideo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsLive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPrivate = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFinished = table.Column<bool>(type: "INTEGER", nullable: false),
                    FinishedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AnalyzeOpponents = table.Column<bool>(type: "INTEGER", nullable: false),
                    AnalyzeBoth = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    JsonConfig = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentPeriod = table.Column<int>(type: "INTEGER", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analyses_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Analyses_Teams_AnalyzedTeamId",
                        column: x => x.AnalyzedTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analyses_Teams_OpponentTeamId",
                        column: x => x.OpponentTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analyses_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamPlayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FieldPositionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    JerseyNumber = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamPlayers_FieldPositions_FieldPositionId",
                        column: x => x.FieldPositionId,
                        principalTable: "FieldPositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamPlayers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisMedias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AnalysisId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    MediaType = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    OffsetMs = table.Column<int>(type: "INTEGER", nullable: true),
                    DurationMs = table.Column<int>(type: "INTEGER", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Thumbnail = table.Column<string>(type: "TEXT", nullable: true),
                    IsUploaded = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisMedias_Analyses_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "Analyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamePeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AnalysisId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PeriodIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    PeriodName = table.Column<string>(type: "TEXT", nullable: false),
                    IsMatchStart = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPeriodStart = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMatchEnd = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePeriods_Analyses_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "Analyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServicePlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AnalysisId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SportDisciplineId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalDurationSec = table.Column<double>(type: "REAL", nullable: true),
                    MediaUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    IsPrivate = table.Column<bool>(type: "INTEGER", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    ShowHeaders = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowNotes = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowSlideNumbers = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowChapters = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowBody = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowCover = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowCredits = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowPlayerOnAction = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowActionTime = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowBadge = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Analyses_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "Analyses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_SportDisciplines_SportDisciplineId",
                        column: x => x.SportDisciplineId,
                        principalTable: "SportDisciplines",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MatchActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AnalysisId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ButtonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: true),
                    GamePeriodId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ButtonName = table.Column<string>(type: "TEXT", nullable: true),
                    ButtonColor = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    MediaUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TimestampEnd = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    TimestampMs = table.Column<double>(type: "REAL", nullable: true),
                    TimestampEndMs = table.Column<double>(type: "REAL", nullable: true),
                    SecondsBeforeClip = table.Column<double>(type: "REAL", nullable: false),
                    SecondsAfterClip = table.Column<double>(type: "REAL", nullable: false),
                    VideoPositionMs = table.Column<int>(type: "INTEGER", nullable: false),
                    VideoStartMs = table.Column<int>(type: "INTEGER", nullable: false),
                    VideoEndMs = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsInOut = table.Column<int>(type: "INTEGER", nullable: true),
                    JsonAttributes = table.Column<string>(type: "TEXT", nullable: true),
                    GeoLocation = table.Column<string>(type: "TEXT", nullable: true),
                    GeoLocationEnd = table.Column<string>(type: "TEXT", nullable: true),
                    Distance = table.Column<string>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: true),
                    SortOrderCreation = table.Column<int>(type: "INTEGER", nullable: true),
                    Score = table.Column<string>(type: "TEXT", nullable: true),
                    ParentActionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RelatedActionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchActions_Analyses_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "Analyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchActions_Buttons_ButtonId",
                        column: x => x.ButtonId,
                        principalTable: "Buttons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchActions_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchActions_GamePeriods_GamePeriodId",
                        column: x => x.GamePeriodId,
                        principalTable: "GamePeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatchActions_MatchActions_ParentActionId",
                        column: x => x.ParentActionId,
                        principalTable: "MatchActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchActions_MatchActions_RelatedActionId",
                        column: x => x.RelatedActionId,
                        principalTable: "MatchActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchActions_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActionConcepts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ActionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConceptId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ButtonId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ConceptName = table.Column<string>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionConcepts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionConcepts_Buttons_ButtonId",
                        column: x => x.ButtonId,
                        principalTable: "Buttons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActionConcepts_Concepts_ConceptId",
                        column: x => x.ConceptId,
                        principalTable: "Concepts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionConcepts_MatchActions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "MatchActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActionPlayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ActionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionPlayers_MatchActions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "MatchActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Slides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReportId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ActionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ButtonId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SlideType = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Duration = table.Column<double>(type: "REAL", nullable: false),
                    ChapterHtml = table.Column<string>(type: "TEXT", nullable: true),
                    BodyHtml = table.Column<string>(type: "TEXT", nullable: true),
                    HeaderHtml = table.Column<string>(type: "TEXT", nullable: true),
                    NoteHtml = table.Column<string>(type: "TEXT", nullable: true),
                    ShowChapter = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowBody = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowHeader = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowNote = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowSlideNumber = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowBackgroundImage = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackgroundImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    JsonDrawings = table.Column<string>(type: "TEXT", nullable: true),
                    JsonZoom = table.Column<string>(type: "TEXT", nullable: true),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsExpanded = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slides_Buttons_ButtonId",
                        column: x => x.ButtonId,
                        principalTable: "Buttons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Slides_MatchActions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "MatchActions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Slides_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Slides_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Slides_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionConcepts_ActionId",
                table: "ActionConcepts",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionConcepts_ButtonId",
                table: "ActionConcepts",
                column: "ButtonId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionConcepts_ConceptId",
                table: "ActionConcepts",
                column: "ConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionPlayers_ActionId",
                table: "ActionPlayers",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionPlayers_PlayerId",
                table: "ActionPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_AnalyzedTeamId",
                table: "Analyses",
                column: "AnalyzedTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_EventId",
                table: "Analyses",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_IsArchived",
                table: "Analyses",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_OpponentTeamId",
                table: "Analyses",
                column: "OpponentTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_SyncStatus",
                table: "Analyses",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_TemplateId",
                table: "Analyses",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisMedias_AnalysisId",
                table: "AnalysisMedias",
                column: "AnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_PanelId",
                table: "Blocks",
                column: "PanelId");

            migrationBuilder.CreateIndex(
                name: "IX_Buttons_BlockId",
                table: "Buttons",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Buttons_ConceptId",
                table: "Buttons",
                column: "ConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_Buttons_ParentButtonId",
                table: "Buttons",
                column: "ParentButtonId");

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_SportCategoryId",
                table: "Competitions",
                column: "SportCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Concepts_CustomerId",
                table: "Concepts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Concepts_PanelId",
                table: "Concepts",
                column: "PanelId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CountryId",
                table: "Customers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId",
                table: "Customers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AwayTeamId",
                table: "Events",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CompetitionId",
                table: "Events",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_HomeTeamId",
                table: "Events",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_IsArchived",
                table: "Events",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_Events_SyncStatus",
                table: "Events",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_FieldPositions_CustomerId",
                table: "FieldPositions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldPositions_SportDisciplineId",
                table: "FieldPositions",
                column: "SportDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePeriods_AnalysisId",
                table: "GamePeriods",
                column: "AnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_AnalysisId",
                table: "MatchActions",
                column: "AnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_ButtonId",
                table: "MatchActions",
                column: "ButtonId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_EventId",
                table: "MatchActions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_GamePeriodId",
                table: "MatchActions",
                column: "GamePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_IsArchived",
                table: "MatchActions",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_ParentActionId",
                table: "MatchActions",
                column: "ParentActionId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_RelatedActionId",
                table: "MatchActions",
                column: "RelatedActionId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_SyncStatus",
                table: "MatchActions",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_TeamId",
                table: "MatchActions",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CountryId",
                table: "Organizations",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_SportDisciplineId",
                table: "Organizations",
                column: "SportDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Panels_TemplateId",
                table: "Panels",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonRoles_SportDisciplineId",
                table: "PersonRoles",
                column: "SportDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CountryId",
                table: "Persons",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CustomerId",
                table: "Persons",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_RoleId",
                table: "Persons",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_SportDisciplineId",
                table: "Persons",
                column: "SportDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_CountryId",
                table: "Players",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_DefaultTeamId",
                table: "Players",
                column: "DefaultTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_FieldPositionId",
                table: "Players",
                column: "FieldPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_OrganizationId",
                table: "Players",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_SecondCountryId",
                table: "Players",
                column: "SecondCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_SportDisciplineId",
                table: "Players",
                column: "SportDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AnalysisId",
                table: "Reports",
                column: "AnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CustomerId",
                table: "Reports",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_EventId",
                table: "Reports",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SportDisciplineId",
                table: "Reports",
                column: "SportDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Slides_ActionId",
                table: "Slides",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_Slides_ButtonId",
                table: "Slides",
                column: "ButtonId");

            migrationBuilder.CreateIndex(
                name: "IX_Slides_PlayerId",
                table: "Slides",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Slides_ReportId",
                table: "Slides",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Slides_TeamId",
                table: "Slides",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SportCategories_CustomerId",
                table: "SportCategories",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SportCategories_SportDisciplineId",
                table: "SportCategories",
                column: "SportDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCustomers_CountryId",
                table: "SubCustomers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCustomers_CustomerId",
                table: "SubCustomers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_FieldPositionId",
                table: "TeamPlayers",
                column: "FieldPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_PlayerId",
                table: "TeamPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_TeamId",
                table: "TeamPlayers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_OrganizationId",
                table: "Teams",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SportCategoryId",
                table: "Teams",
                column: "SportCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_SportDisciplineId",
                table: "Templates",
                column: "SportDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_IsArchived",
                table: "Tenants",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_SyncStatus",
                table: "Tenants",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CountryId",
                table: "Users",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CustomerId",
                table: "Users",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ServicePlanId",
                table: "Users",
                column: "ServicePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SubCustomerId",
                table: "Users",
                column: "SubCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionConcepts");

            migrationBuilder.DropTable(
                name: "ActionPlayers");

            migrationBuilder.DropTable(
                name: "AnalysisMedias");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Slides");

            migrationBuilder.DropTable(
                name: "SyncLogs");

            migrationBuilder.DropTable(
                name: "TeamPlayers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "PersonRoles");

            migrationBuilder.DropTable(
                name: "MatchActions");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "ServicePlans");

            migrationBuilder.DropTable(
                name: "SubCustomers");

            migrationBuilder.DropTable(
                name: "Buttons");

            migrationBuilder.DropTable(
                name: "GamePeriods");

            migrationBuilder.DropTable(
                name: "FieldPositions");

            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "Concepts");

            migrationBuilder.DropTable(
                name: "Analyses");

            migrationBuilder.DropTable(
                name: "Panels");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "Competitions");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "SportCategories");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "SportDisciplines");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
