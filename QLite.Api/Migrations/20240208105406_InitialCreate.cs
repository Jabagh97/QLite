using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLiteDataApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Mail = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LogoS = table.Column<byte[]>(type: "BLOB", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Mask = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: true),
                    PhoneCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LangCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Logo = table.Column<byte[]>(type: "BLOB", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "Design",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DesignData = table.Column<string>(type: "TEXT", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    KioskApplicationType = table.Column<Guid>(type: "TEXT", nullable: true),
                    DesignTag = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    WfStep = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Design", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "DeskStatus",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Desk = table.Column<Guid>(type: "TEXT", nullable: true),
                    User = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeskActivityStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    StateStartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    StateEndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskStatus", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "KappSessionStep",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Step = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SubStep = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    KappSession = table.Column<Guid>(type: "TEXT", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KappSessionStep", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "KappWorkflow",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SessionType = table.Column<string>(type: "TEXT", nullable: true),
                    DesignData = table.Column<string>(type: "TEXT", nullable: true),
                    RestartProfile = table.Column<Guid>(type: "TEXT", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KappWorkflow", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EnglishName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LocalName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CultureInfo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LangCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Logo = table.Column<byte[]>(type: "BLOB", nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "RestartProfile",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RestartPerNumOfSession = table.Column<int>(type: "INTEGER", nullable: true),
                    RestartPerNumOfDays = table.Column<int>(type: "INTEGER", nullable: true),
                    RestartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestartProfile", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "UploadBO",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadBO", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "KioskApplicationType",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    QorchAppType = table.Column<int>(type: "INTEGER", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KioskApplicationType", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_KioskApplicationType_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "Macro",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    MacroType = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ToThisDesk = table.Column<int>(type: "INTEGER", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxWaitingTime = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Macro", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_Macro_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "Segment",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Default = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsParent = table.Column<bool>(type: "INTEGER", nullable: true),
                    Parent = table.Column<Guid>(type: "TEXT", nullable: true),
                    Prefix = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Segment", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_Segment_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Segment_Parent",
                        column: x => x.Parent,
                        principalTable: "Segment",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "ServiceType",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Icon = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Parent = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsParent = table.Column<bool>(type: "INTEGER", nullable: true),
                    CallInKiosk = table.Column<bool>(type: "INTEGER", nullable: true),
                    GenTicketByDesk = table.Column<bool>(type: "INTEGER", nullable: true),
                    Default = table.Column<bool>(type: "INTEGER", nullable: true),
                    SeqNo = table.Column<int>(type: "INTEGER", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceType", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_ServiceType_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_ServiceType_Parent",
                        column: x => x.Parent,
                        principalTable: "ServiceType",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "TicketPoolProfile",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketPoolProfile", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_TicketPoolProfile_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "Province",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Country = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Province", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_Province_Country",
                        column: x => x.Country,
                        principalTable: "Country",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "DesignTarget",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Design = table.Column<Guid>(type: "TEXT", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Branch = table.Column<Guid>(type: "TEXT", nullable: true),
                    KioskApplication = table.Column<Guid>(type: "TEXT", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignTarget", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_DesignTarget_Design",
                        column: x => x.Design,
                        principalTable: "Design",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "AccountLanguage",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Language = table.Column<Guid>(type: "TEXT", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLanguage", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_AccountLanguage_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_AccountLanguage_Language",
                        column: x => x.Language,
                        principalTable: "Language",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Language = table.Column<Guid>(type: "TEXT", nullable: true),
                    Parameter = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ParameterValue = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_Resource_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Resource_Language",
                        column: x => x.Language,
                        principalTable: "Language",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "MacroRule",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Macro = table.Column<Guid>(type: "TEXT", nullable: true),
                    ServiceType = table.Column<Guid>(type: "TEXT", nullable: true),
                    Segment = table.Column<Guid>(type: "TEXT", nullable: true),
                    Transfer = table.Column<bool>(type: "INTEGER", nullable: true),
                    ToThisDesk = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxWaitingTime = table.Column<int>(type: "INTEGER", nullable: true),
                    MinWaitingTime = table.Column<int>(type: "INTEGER", nullable: true),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MacroRule", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_MacroRule_Macro",
                        column: x => x.Macro,
                        principalTable: "Macro",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_MacroRule_Segment",
                        column: x => x.Segment,
                        principalTable: "Segment",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_MacroRule_ServiceType",
                        column: x => x.ServiceType,
                        principalTable: "ServiceType",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "SubProvince",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Country = table.Column<Guid>(type: "TEXT", nullable: true),
                    Province = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProvince", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_SubProvince_Country",
                        column: x => x.Country,
                        principalTable: "Country",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_SubProvince_Province",
                        column: x => x.Province,
                        principalTable: "Province",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "Branch",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Country = table.Column<Guid>(type: "TEXT", nullable: true),
                    Province = table.Column<Guid>(type: "TEXT", nullable: true),
                    SubProvince = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    BranchCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Terminal = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Area = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Address2 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TicketPoolProfile = table.Column<Guid>(type: "TEXT", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_Branch_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Branch_Country",
                        column: x => x.Country,
                        principalTable: "Country",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Branch_Province",
                        column: x => x.Province,
                        principalTable: "Province",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Branch_SubProvince",
                        column: x => x.SubProvince,
                        principalTable: "SubProvince",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Branch_TicketPoolProfile",
                        column: x => x.TicketPoolProfile,
                        principalTable: "TicketPoolProfile",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "KioskApplication",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Branch = table.Column<Guid>(type: "TEXT", nullable: true),
                    KioskApplicationType = table.Column<Guid>(type: "TEXT", nullable: true),
                    KappWorkflow = table.Column<Guid>(type: "TEXT", nullable: true),
                    KappName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    HwId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DesignTag = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PlatformAuthClientId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PlatformAuthClientSecret = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: true),
                    HasDigitalDisplay = table.Column<bool>(type: "INTEGER", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KioskApplication", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_KioskApplication_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_KioskApplication_Branch",
                        column: x => x.Branch,
                        principalTable: "Branch",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_KioskApplication_KappWorkflow",
                        column: x => x.KappWorkflow,
                        principalTable: "KappWorkflow",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_KioskApplication_KioskApplicationType",
                        column: x => x.KioskApplicationType,
                        principalTable: "KioskApplicationType",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "Desk",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Branch = table.Column<Guid>(type: "TEXT", nullable: true),
                    Pano = table.Column<Guid>(type: "TEXT", nullable: true),
                    DisplayNo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ActivityStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    CurrentTicketNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    LastStateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    Autocall = table.Column<bool>(type: "INTEGER", nullable: true),
                    ActiveUser = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desk", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_Desk_Branch",
                        column: x => x.Branch,
                        principalTable: "Branch",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Desk_Pano",
                        column: x => x.Pano,
                        principalTable: "KioskApplication",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "KappRelation",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Parent = table.Column<Guid>(type: "TEXT", nullable: true),
                    Child = table.Column<Guid>(type: "TEXT", nullable: true),
                    Icon = table.Column<byte[]>(type: "BLOB", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KappRelation", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_KappRelation_Child",
                        column: x => x.Child,
                        principalTable: "KioskApplication",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_KappRelation_Parent",
                        column: x => x.Parent,
                        principalTable: "KioskApplication",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "KappSettings",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Branch = table.Column<Guid>(type: "TEXT", nullable: true),
                    KioskApplication = table.Column<Guid>(type: "TEXT", nullable: true),
                    Parameter = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ParameterValue = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CacheTimeout = table.Column<int>(type: "INTEGER", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KappSettings", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_KappSettings_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_KappSettings_Branch",
                        column: x => x.Branch,
                        principalTable: "Branch",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_KappSettings_KioskApplication",
                        column: x => x.KioskApplication,
                        principalTable: "KioskApplication",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "QorchSession",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    KioskApplication = table.Column<Guid>(type: "TEXT", nullable: true),
                    Segment = table.Column<Guid>(type: "TEXT", nullable: true),
                    ServiceType = table.Column<Guid>(type: "TEXT", nullable: true),
                    Success = table.Column<bool>(type: "INTEGER", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    StartTimeUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndTimeUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    CurrentStep = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    InputValue = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    InputInfo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    InputType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Workflow = table.Column<Guid>(type: "TEXT", nullable: true),
                    Error = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QorchSession", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_QorchSession_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_QorchSession_KioskApplication",
                        column: x => x.KioskApplication,
                        principalTable: "KioskApplication",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_QorchSession_Segment",
                        column: x => x.Segment,
                        principalTable: "Segment",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_QorchSession_ServiceType",
                        column: x => x.ServiceType,
                        principalTable: "ServiceType",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "TicketPool",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Branch = table.Column<Guid>(type: "TEXT", nullable: true),
                    TicketPoolProfile = table.Column<Guid>(type: "TEXT", nullable: true),
                    ServiceType = table.Column<Guid>(type: "TEXT", nullable: true),
                    Segment = table.Column<Guid>(type: "TEXT", nullable: true),
                    KioskApplication = table.Column<Guid>(type: "TEXT", nullable: true),
                    MaxWaitingTicketCount = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxWaitingTicketCountControlTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ServiceStartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ServiceEndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    BreakStartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    BreakEndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    RangeStart = table.Column<int>(type: "INTEGER", nullable: true),
                    RangeEnd = table.Column<int>(type: "INTEGER", nullable: true),
                    ResetOnRange = table.Column<bool>(type: "INTEGER", nullable: true),
                    NotAvailable = table.Column<bool>(type: "INTEGER", nullable: true),
                    ServiceCode = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true),
                    CopyNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketPool", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_TicketPool_Account",
                        column: x => x.Account,
                        principalTable: "Account",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_TicketPool_Branch",
                        column: x => x.Branch,
                        principalTable: "Branch",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_TicketPool_KioskApplication",
                        column: x => x.KioskApplication,
                        principalTable: "KioskApplication",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_TicketPool_Segment",
                        column: x => x.Segment,
                        principalTable: "Segment",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_TicketPool_ServiceType",
                        column: x => x.ServiceType,
                        principalTable: "ServiceType",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_TicketPool_TicketPoolProfile",
                        column: x => x.TicketPoolProfile,
                        principalTable: "TicketPoolProfile",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "DeskCreatableServices",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Branch = table.Column<Guid>(type: "TEXT", nullable: true),
                    Desk = table.Column<Guid>(type: "TEXT", nullable: true),
                    ServiceType = table.Column<Guid>(type: "TEXT", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskCreatableServices", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_DeskCreatableServices_Branch",
                        column: x => x.Branch,
                        principalTable: "Branch",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_DeskCreatableServices_Desk",
                        column: x => x.Desk,
                        principalTable: "Desk",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_DeskCreatableServices_ServiceType",
                        column: x => x.ServiceType,
                        principalTable: "ServiceType",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "DeskMacroSchedule",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Macro = table.Column<Guid>(type: "TEXT", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Branch = table.Column<Guid>(type: "TEXT", nullable: true),
                    Desk = table.Column<Guid>(type: "TEXT", nullable: true),
                    User = table.Column<Guid>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    HaftalikRutin = table.Column<bool>(type: "INTEGER", nullable: true),
                    D1 = table.Column<bool>(type: "INTEGER", nullable: true),
                    D2 = table.Column<bool>(type: "INTEGER", nullable: true),
                    D3 = table.Column<bool>(type: "INTEGER", nullable: true),
                    D4 = table.Column<bool>(type: "INTEGER", nullable: true),
                    D5 = table.Column<bool>(type: "INTEGER", nullable: true),
                    D6 = table.Column<bool>(type: "INTEGER", nullable: true),
                    D7 = table.Column<bool>(type: "INTEGER", nullable: true),
                    Pasif = table.Column<bool>(type: "INTEGER", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskMacroSchedule", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_DeskMacroSchedule_Branch",
                        column: x => x.Branch,
                        principalTable: "Branch",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_DeskMacroSchedule_Desk",
                        column: x => x.Desk,
                        principalTable: "Desk",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_DeskMacroSchedule_Macro",
                        column: x => x.Macro,
                        principalTable: "Macro",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "DeskTransferableServices",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Account = table.Column<Guid>(type: "TEXT", nullable: true),
                    Branch = table.Column<Guid>(type: "TEXT", nullable: true),
                    Desk = table.Column<Guid>(type: "TEXT", nullable: true),
                    ServiceType = table.Column<Guid>(type: "TEXT", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskTransferableServices", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_DeskTransferableServices_Branch",
                        column: x => x.Branch,
                        principalTable: "Branch",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_DeskTransferableServices_Desk",
                        column: x => x.Desk,
                        principalTable: "Desk",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_DeskTransferableServices_ServiceType",
                        column: x => x.ServiceType,
                        principalTable: "ServiceType",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Branch = table.Column<Guid>(type: "TEXT", nullable: true),
                    ServiceType = table.Column<Guid>(type: "TEXT", nullable: true),
                    Desk = table.Column<Guid>(type: "TEXT", nullable: true),
                    Segment = table.Column<Guid>(type: "TEXT", nullable: true),
                    ServiceTypeName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SegmentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LangCode = table.Column<string>(type: "TEXT", maxLength: 4, nullable: true),
                    CurrentState = table.Column<int>(type: "INTEGER", nullable: true),
                    LastOpr = table.Column<int>(type: "INTEGER", nullable: true),
                    LastOprTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ToServiceType = table.Column<Guid>(type: "TEXT", nullable: true),
                    ToDesk = table.Column<Guid>(type: "TEXT", nullable: true),
                    CurrentDesk = table.Column<Guid>(type: "TEXT", nullable: true),
                    Number = table.Column<int>(type: "INTEGER", nullable: true),
                    DayOfYear = table.Column<int>(type: "INTEGER", nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: true),
                    TicketNote = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CustomerInfo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CustomerNo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CardNo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NationalId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TicketPool = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedByDesk = table.Column<Guid>(type: "TEXT", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedByKiosk = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_Ticket_Branch",
                        column: x => x.Branch,
                        principalTable: "Branch",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Ticket_CurrentDesk",
                        column: x => x.CurrentDesk,
                        principalTable: "Desk",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Ticket_Desk",
                        column: x => x.Desk,
                        principalTable: "Desk",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Ticket_Segment",
                        column: x => x.Segment,
                        principalTable: "Segment",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Ticket_ServiceType",
                        column: x => x.ServiceType,
                        principalTable: "ServiceType",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Ticket_ToDesk",
                        column: x => x.ToDesk,
                        principalTable: "Desk",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_Ticket_ToServiceType",
                        column: x => x.ToServiceType,
                        principalTable: "ServiceType",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "TicketState",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Desk = table.Column<Guid>(type: "TEXT", nullable: true),
                    User = table.Column<Guid>(type: "TEXT", nullable: true),
                    Ticket = table.Column<Guid>(type: "TEXT", nullable: true),
                    Branch = table.Column<Guid>(type: "TEXT", nullable: true),
                    TicketNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    TicketStateValue = table.Column<int>(type: "INTEGER", nullable: true),
                    TicketOprValue = table.Column<int>(type: "INTEGER", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ServiceType = table.Column<Guid>(type: "TEXT", nullable: true),
                    Segment = table.Column<Guid>(type: "TEXT", nullable: true),
                    ServiceTypeName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SegmentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Macro = table.Column<Guid>(type: "TEXT", nullable: true),
                    CallingRuleDescription = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    DeskAppType = table.Column<int>(type: "INTEGER", nullable: true),
                    TicketCallType = table.Column<int>(type: "INTEGER", nullable: true),
                    OptimisticLockField = table.Column<int>(type: "INTEGER", nullable: true),
                    GCRecord = table.Column<int>(type: "INTEGER", nullable: true),
                    KioskAppID = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketState", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_TicketState_Branch",
                        column: x => x.Branch,
                        principalTable: "Branch",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_TicketState_Desk",
                        column: x => x.Desk,
                        principalTable: "Desk",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_TicketState_Segment",
                        column: x => x.Segment,
                        principalTable: "Segment",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_TicketState_ServiceType",
                        column: x => x.ServiceType,
                        principalTable: "ServiceType",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_TicketState_Ticket",
                        column: x => x.Ticket,
                        principalTable: "Ticket",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateIndex(
                name: "iGCRecord_Account",
                table: "Account",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iAccount_AccountLanguage",
                table: "AccountLanguage",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_AccountLanguage",
                table: "AccountLanguage",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iLanguage_AccountLanguage",
                table: "AccountLanguage",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "iAccount_Branch",
                table: "Branch",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iCountry_Branch",
                table: "Branch",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_Branch",
                table: "Branch",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iProvince_Branch",
                table: "Branch",
                column: "Province");

            migrationBuilder.CreateIndex(
                name: "iSubProvince_Branch",
                table: "Branch",
                column: "SubProvince");

            migrationBuilder.CreateIndex(
                name: "iTicketPoolProfile_Branch",
                table: "Branch",
                column: "TicketPoolProfile");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_Country",
                table: "Country",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_Design",
                table: "Design",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iDesign_DesignTarget",
                table: "DesignTarget",
                column: "Design");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_DesignTarget",
                table: "DesignTarget",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iBranch_Desk",
                table: "Desk",
                column: "Branch");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_Desk",
                table: "Desk",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iPano_Desk",
                table: "Desk",
                column: "Pano");

            migrationBuilder.CreateIndex(
                name: "iBranch_DeskCreatableServices",
                table: "DeskCreatableServices",
                column: "Branch");

            migrationBuilder.CreateIndex(
                name: "iDesk_DeskCreatableServices",
                table: "DeskCreatableServices",
                column: "Desk");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_DeskCreatableServices",
                table: "DeskCreatableServices",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iServiceType_DeskCreatableServices",
                table: "DeskCreatableServices",
                column: "ServiceType");

            migrationBuilder.CreateIndex(
                name: "iBranch_DeskMacroSchedule",
                table: "DeskMacroSchedule",
                column: "Branch");

            migrationBuilder.CreateIndex(
                name: "iDesk_DeskMacroSchedule",
                table: "DeskMacroSchedule",
                column: "Desk");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_DeskMacroSchedule",
                table: "DeskMacroSchedule",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iMacro_DeskMacroSchedule",
                table: "DeskMacroSchedule",
                column: "Macro");

            migrationBuilder.CreateIndex(
                name: "iUser_DeskMacroSchedule",
                table: "DeskMacroSchedule",
                column: "User");

            migrationBuilder.CreateIndex(
                name: "iDesk_DeskStatus",
                table: "DeskStatus",
                column: "Desk");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_DeskStatus",
                table: "DeskStatus",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iUser_DeskStatus",
                table: "DeskStatus",
                column: "User");

            migrationBuilder.CreateIndex(
                name: "iBranch_DeskTransferableServices",
                table: "DeskTransferableServices",
                column: "Branch");

            migrationBuilder.CreateIndex(
                name: "iDesk_DeskTransferableServices",
                table: "DeskTransferableServices",
                column: "Desk");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_DeskTransferableServices",
                table: "DeskTransferableServices",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iServiceType_DeskTransferableServices",
                table: "DeskTransferableServices",
                column: "ServiceType");

            migrationBuilder.CreateIndex(
                name: "iChild_KappRelation",
                table: "KappRelation",
                column: "Child");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_KappRelation",
                table: "KappRelation",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iParent_KappRelation",
                table: "KappRelation",
                column: "Parent");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_KappSessionStep",
                table: "KappSessionStep",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iAccount_KappSettings",
                table: "KappSettings",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iBranch_KappSettings",
                table: "KappSettings",
                column: "Branch");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_KappSettings",
                table: "KappSettings",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iKioskApplication_KappSettings",
                table: "KappSettings",
                column: "KioskApplication");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_KappWorkflow",
                table: "KappWorkflow",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iAccount_KioskApplication",
                table: "KioskApplication",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iBranch_KioskApplication",
                table: "KioskApplication",
                column: "Branch");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_KioskApplication",
                table: "KioskApplication",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iKappWorkflow_KioskApplication",
                table: "KioskApplication",
                column: "KappWorkflow");

            migrationBuilder.CreateIndex(
                name: "iKioskApplicationType_KioskApplication",
                table: "KioskApplication",
                column: "KioskApplicationType");

            migrationBuilder.CreateIndex(
                name: "iAccount_KioskApplicationType",
                table: "KioskApplicationType",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_KioskApplicationType",
                table: "KioskApplicationType",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_Language",
                table: "Language",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iAccount_Macro",
                table: "Macro",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_Macro",
                table: "Macro",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_MacroRule",
                table: "MacroRule",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iMacro_MacroRule",
                table: "MacroRule",
                column: "Macro");

            migrationBuilder.CreateIndex(
                name: "iSegment_MacroRule",
                table: "MacroRule",
                column: "Segment");

            migrationBuilder.CreateIndex(
                name: "iServiceType_MacroRule",
                table: "MacroRule",
                column: "ServiceType");

            migrationBuilder.CreateIndex(
                name: "iCountry_Province",
                table: "Province",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_Province",
                table: "Province",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iAccount_QorchSession",
                table: "QorchSession",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_QorchSession",
                table: "QorchSession",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iKioskApplication_QorchSession",
                table: "QorchSession",
                column: "KioskApplication");

            migrationBuilder.CreateIndex(
                name: "iSegment_QorchSession",
                table: "QorchSession",
                column: "Segment");

            migrationBuilder.CreateIndex(
                name: "iServiceType_QorchSession",
                table: "QorchSession",
                column: "ServiceType");

            migrationBuilder.CreateIndex(
                name: "iAccount_Resource",
                table: "Resource",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_Resource",
                table: "Resource",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iLanguage_Resource",
                table: "Resource",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_RestartProfile",
                table: "RestartProfile",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iAccount_Segment",
                table: "Segment",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_Segment",
                table: "Segment",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iParent_Segment",
                table: "Segment",
                column: "Parent");

            migrationBuilder.CreateIndex(
                name: "iAccount_ServiceType",
                table: "ServiceType",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_ServiceType",
                table: "ServiceType",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iParent_ServiceType",
                table: "ServiceType",
                column: "Parent");

            migrationBuilder.CreateIndex(
                name: "iCountry_SubProvince",
                table: "SubProvince",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_SubProvince",
                table: "SubProvince",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iProvince_SubProvince",
                table: "SubProvince",
                column: "Province");

            migrationBuilder.CreateIndex(
                name: "iBranch_Ticket",
                table: "Ticket",
                column: "Branch");

            migrationBuilder.CreateIndex(
                name: "iCurrentDesk_Ticket",
                table: "Ticket",
                column: "CurrentDesk");

            migrationBuilder.CreateIndex(
                name: "iDesk_Ticket",
                table: "Ticket",
                column: "Desk");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_Ticket",
                table: "Ticket",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iSegment_Ticket",
                table: "Ticket",
                column: "Segment");

            migrationBuilder.CreateIndex(
                name: "iServiceType_Ticket",
                table: "Ticket",
                column: "ServiceType");

            migrationBuilder.CreateIndex(
                name: "iToDesk_Ticket",
                table: "Ticket",
                column: "ToDesk");

            migrationBuilder.CreateIndex(
                name: "iToServiceType_Ticket",
                table: "Ticket",
                column: "ToServiceType");

            migrationBuilder.CreateIndex(
                name: "iAccount_TicketPool",
                table: "TicketPool",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iBranch_TicketPool",
                table: "TicketPool",
                column: "Branch");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_TicketPool",
                table: "TicketPool",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iKioskApplication_TicketPool",
                table: "TicketPool",
                column: "KioskApplication");

            migrationBuilder.CreateIndex(
                name: "iSegment_TicketPool",
                table: "TicketPool",
                column: "Segment");

            migrationBuilder.CreateIndex(
                name: "iServiceType_TicketPool",
                table: "TicketPool",
                column: "ServiceType");

            migrationBuilder.CreateIndex(
                name: "iTicketPoolProfile_TicketPool",
                table: "TicketPool",
                column: "TicketPoolProfile");

            migrationBuilder.CreateIndex(
                name: "iAccount_TicketPoolProfile",
                table: "TicketPoolProfile",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_TicketPoolProfile",
                table: "TicketPoolProfile",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iBranch_TicketState",
                table: "TicketState",
                column: "Branch");

            migrationBuilder.CreateIndex(
                name: "iDesk_TicketState",
                table: "TicketState",
                column: "Desk");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_TicketState",
                table: "TicketState",
                column: "GCRecord");

            migrationBuilder.CreateIndex(
                name: "iSegment_TicketState",
                table: "TicketState",
                column: "Segment");

            migrationBuilder.CreateIndex(
                name: "iServiceType_TicketState",
                table: "TicketState",
                column: "ServiceType");

            migrationBuilder.CreateIndex(
                name: "iTicket_TicketState",
                table: "TicketState",
                column: "Ticket");

            migrationBuilder.CreateIndex(
                name: "iGCRecord_UploadBO",
                table: "UploadBO",
                column: "GCRecord");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountLanguage");

            migrationBuilder.DropTable(
                name: "DesignTarget");

            migrationBuilder.DropTable(
                name: "DeskCreatableServices");

            migrationBuilder.DropTable(
                name: "DeskMacroSchedule");

            migrationBuilder.DropTable(
                name: "DeskStatus");

            migrationBuilder.DropTable(
                name: "DeskTransferableServices");

            migrationBuilder.DropTable(
                name: "KappRelation");

            migrationBuilder.DropTable(
                name: "KappSessionStep");

            migrationBuilder.DropTable(
                name: "KappSettings");

            migrationBuilder.DropTable(
                name: "MacroRule");

            migrationBuilder.DropTable(
                name: "QorchSession");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropTable(
                name: "RestartProfile");

            migrationBuilder.DropTable(
                name: "TicketPool");

            migrationBuilder.DropTable(
                name: "TicketState");

            migrationBuilder.DropTable(
                name: "UploadBO");

            migrationBuilder.DropTable(
                name: "Design");

            migrationBuilder.DropTable(
                name: "Macro");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Desk");

            migrationBuilder.DropTable(
                name: "Segment");

            migrationBuilder.DropTable(
                name: "ServiceType");

            migrationBuilder.DropTable(
                name: "KioskApplication");

            migrationBuilder.DropTable(
                name: "Branch");

            migrationBuilder.DropTable(
                name: "KappWorkflow");

            migrationBuilder.DropTable(
                name: "KioskApplicationType");

            migrationBuilder.DropTable(
                name: "SubProvince");

            migrationBuilder.DropTable(
                name: "TicketPoolProfile");

            migrationBuilder.DropTable(
                name: "Province");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Country");
        }
    }
}
