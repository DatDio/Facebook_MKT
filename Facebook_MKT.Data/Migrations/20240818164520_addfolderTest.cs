using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facebook_MKT.Data.Migrations
{
    /// <inheritdoc />
    public partial class addfolderTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Folders",
                columns: new[] { "FolderIdKey", "FolderName" },
                values: new object[] { 2, "Test" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Folders",
                keyColumn: "FolderIdKey",
                keyValue: 2);
        }
    }
}
