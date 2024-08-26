using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facebook_MKT.Data.Migrations
{
    /// <inheritdoc />
    public partial class initinial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    FolderIdKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FolderName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.FolderIdKey);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountIDKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UID = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Email1 = table.Column<string>(type: "TEXT", nullable: false),
                    Email1Password = table.Column<string>(type: "TEXT", nullable: false),
                    Email2 = table.Column<string>(type: "TEXT", nullable: false),
                    Email2Password = table.Column<string>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Cookie = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Proxy = table.Column<string>(type: "TEXT", nullable: false),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: false),
                    GPMID = table.Column<string>(type: "TEXT", nullable: false),
                    FolderIdKey = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountIDKey);
                    table.ForeignKey(
                        name: "FK_Accounts_Folders_FolderIdKey",
                        column: x => x.FolderIdKey,
                        principalTable: "Folders",
                        principalColumn: "FolderIdKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    PageIdKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageID = table.Column<string>(type: "TEXT", nullable: true),
                    AccountID = table.Column<int>(type: "INTEGER", nullable: false),
                    PageName = table.Column<string>(type: "TEXT", nullable: false),
                    PageFollow = table.Column<string>(type: "TEXT", nullable: false),
                    PageLike = table.Column<string>(type: "TEXT", nullable: false),
                    PageStatus = table.Column<string>(type: "TEXT", nullable: false),
                    FolderIdKey = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.PageIdKey);
                    table.ForeignKey(
                        name: "FK_Pages_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountIDKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pages_Folders_FolderIdKey",
                        column: x => x.FolderIdKey,
                        principalTable: "Folders",
                        principalColumn: "FolderIdKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_FolderIdKey",
                table: "Accounts",
                column: "FolderIdKey");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_AccountID",
                table: "Pages",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_FolderIdKey",
                table: "Pages",
                column: "FolderIdKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Folders");
        }
    }
}
