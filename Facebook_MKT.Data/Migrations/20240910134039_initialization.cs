using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Facebook_MKT.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FolderGroup",
                columns: table => new
                {
                    FolderIdKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FolderName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderGroup", x => x.FolderIdKey);
                });

            migrationBuilder.CreateTable(
                name: "FolderPage",
                columns: table => new
                {
                    FolderIdKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FolderName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderPage", x => x.FolderIdKey);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    FolderIdKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FolderName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.FolderIdKey);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    GroupIdKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupID = table.Column<string>(type: "TEXT", nullable: true),
                    AccountID = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupName = table.Column<string>(type: "TEXT", nullable: false),
                    GroupMember = table.Column<string>(type: "TEXT", nullable: true),
                    GroupCensor = table.Column<bool>(type: "INTEGER", nullable: true),
                    GroupStatus = table.Column<string>(type: "TEXT", nullable: true),
                    FolderIdKey = table.Column<int>(type: "INTEGER", nullable: false),
                    FolderGroupFolderIdKey = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.GroupIdKey);
                    table.ForeignKey(
                        name: "FK_Group_FolderGroup_FolderGroupFolderIdKey",
                        column: x => x.FolderGroupFolderIdKey,
                        principalTable: "FolderGroup",
                        principalColumn: "FolderIdKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountIDKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UID = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    C_2FA = table.Column<string>(type: "TEXT", nullable: true),
                    Email1 = table.Column<string>(type: "TEXT", nullable: true),
                    Email1Password = table.Column<string>(type: "TEXT", nullable: true),
                    Email2 = table.Column<string>(type: "TEXT", nullable: true),
                    Email2Password = table.Column<string>(type: "TEXT", nullable: true),
                    Token = table.Column<string>(type: "TEXT", nullable: true),
                    Cookie = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Proxy = table.Column<string>(type: "TEXT", nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: true),
                    GPMID = table.Column<string>(type: "TEXT", nullable: true),
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
                    PageID = table.Column<string>(type: "TEXT", nullable: false),
                    AccountIDKey = table.Column<int>(type: "INTEGER", nullable: false),
                    PageName = table.Column<string>(type: "TEXT", nullable: false),
                    PageFollow = table.Column<string>(type: "TEXT", nullable: true),
                    PageLike = table.Column<string>(type: "TEXT", nullable: true),
                    PageStatus = table.Column<string>(type: "TEXT", nullable: true),
                    FolderIdKey = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.PageIdKey);
                    table.ForeignKey(
                        name: "FK_Pages_Accounts_AccountIDKey",
                        column: x => x.AccountIDKey,
                        principalTable: "Accounts",
                        principalColumn: "AccountIDKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pages_FolderPage_FolderIdKey",
                        column: x => x.FolderIdKey,
                        principalTable: "FolderPage",
                        principalColumn: "FolderIdKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FolderGroup",
                columns: new[] { "FolderIdKey", "FolderName" },
                values: new object[] { 1, "All" });

            migrationBuilder.InsertData(
                table: "FolderPage",
                columns: new[] { "FolderIdKey", "FolderName" },
                values: new object[] { 1, "All" });

            migrationBuilder.InsertData(
                table: "Folders",
                columns: new[] { "FolderIdKey", "FolderName" },
                values: new object[,]
                {
                    { 1, "All" },
                    { 2, "Test" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountIDKey", "C_2FA", "Cookie", "Email1", "Email1Password", "Email2", "Email2Password", "FolderIdKey", "GPMID", "Password", "Proxy", "Status", "Token", "UID", "UserAgent" },
                values: new object[,]
                {
                    { 1, "eghjdsjkgsdhg", null, null, null, null, null, 1, null, "qưerfuhsdiuvsd", null, null, null, "47812389", null },
                    { 2, "eghjdsjksgsdggsdhg", null, null, null, null, null, 1, null, "passcfb", null, null, null, "4781238923532", null }
                });

            migrationBuilder.InsertData(
                table: "Pages",
                columns: new[] { "PageIdKey", "AccountIDKey", "FolderIdKey", "PageFollow", "PageID", "PageLike", "PageName", "PageStatus" },
                values: new object[,]
                {
                    { 1, 1, 1, null, "23784589235", null, "Dio Tech", null },
                    { 2, 1, 1, null, "23784589233455", null, "Dio Tech", null },
                    { 3, 1, 1, null, "2378454353289235", null, "Vũ trụ bao la", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_FolderIdKey",
                table: "Accounts",
                column: "FolderIdKey");

            migrationBuilder.CreateIndex(
                name: "IX_FolderGroup_FolderName",
                table: "FolderGroup",
                column: "FolderName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FolderPage_FolderName",
                table: "FolderPage",
                column: "FolderName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Folders_FolderName",
                table: "Folders",
                column: "FolderName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Group_FolderGroupFolderIdKey",
                table: "Group",
                column: "FolderGroupFolderIdKey");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_AccountIDKey",
                table: "Pages",
                column: "AccountIDKey");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_FolderIdKey",
                table: "Pages",
                column: "FolderIdKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "FolderGroup");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "FolderPage");

            migrationBuilder.DropTable(
                name: "Folders");
        }
    }
}
