using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Facebook_MKT.Data.Migrations
{
    /// <inheritdoc />
    public partial class addDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Folders",
                keyColumn: "FolderIdKey",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Pages",
                keyColumn: "PageIdKey",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pages",
                keyColumn: "PageIdKey",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Pages",
                keyColumn: "PageIdKey",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Pages",
                keyColumn: "PageIdKey",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountIDKey",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountIDKey",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountIDKey", "C_2FA", "Cookie", "Email1", "Email1Password", "Email2", "Email2Password", "FolderIdKey", "GPMID", "Password", "Proxy", "Status", "Token", "UID", "UserAgent" },
                values: new object[,]
                {
                    { 1, "eghjdsjkgsdhg", null, null, null, null, null, 1, null, "qưerfuhsdiuvsd", null, null, null, "47812389", null },
                    { 2, "eghjdsjksgsdggsdhg", null, null, null, null, null, 1, null, "passcfb", null, null, null, "4781238923532", null }
                });

            migrationBuilder.InsertData(
                table: "Folders",
                columns: new[] { "FolderIdKey", "FolderName" },
                values: new object[] { 2, "Test" });

            migrationBuilder.InsertData(
                table: "Pages",
                columns: new[] { "PageIdKey", "AccountIDKey", "FolderIdKey", "PageFollow", "PageID", "PageLike", "PageName", "PageStatus" },
                values: new object[,]
                {
                    { 1, 1, 1, null, "23784589235", null, "Dio Tech", null },
                    { 2, 1, 1, null, "23784589233455", null, "Dio Tech", null },
                    { 3, 1, 1, null, "2378454353289235", null, "Vũ trụ bao la", null },
                    { 4, 2, 1, null, "2378458923353455", null, "Dio Tech account 2", null }
                });
        }
    }
}
