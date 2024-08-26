using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Facebook_MKT.Data.Migrations
{
    /// <inheritdoc />
    public partial class seedDataToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountIDKey", "C_2FA", "Cookie", "Email1", "Email1Password", "Email2", "Email2Password", "FolderIdKey", "GPMID", "Password", "Proxy", "Status", "Token", "UID", "UserAgent" },
                values: new object[,]
                {
                    { 1, "eghjdsjkgsdhg", null, null, null, null, null, 1, null, "qưerfuhsdiuvsd", null, null, null, "47812389", null },
                    { 2, "eghjdsjksgsdggsdhg", null, null, null, null, null, 1, null, "passcfb", null, null, null, "47812389sdfgsdg", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountIDKey",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountIDKey",
                keyValue: 2);
        }
    }
}
