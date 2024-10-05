using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facebook_MKT.Data.Migrations
{
    /// <inheritdoc />
    public partial class addUniqueForTableAcount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GroupID",
                table: "Group",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Pages",
                columns: new[] { "PageIdKey", "AccountIDKey", "FolderIdKey", "PageFollow", "PageID", "PageLike", "PageName", "PageStatus" },
                values: new object[] { 4, 2, 1, null, "2378458923353455", null, "Dio Tech account 2", null });

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PageID",
                table: "Pages",
                column: "PageID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Group_GroupID",
                table: "Group",
                column: "GroupID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UID",
                table: "Accounts",
                column: "UID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pages_PageID",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Group_GroupID",
                table: "Group");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_UID",
                table: "Accounts");

            migrationBuilder.DeleteData(
                table: "Pages",
                keyColumn: "PageIdKey",
                keyValue: 4);

            migrationBuilder.AlterColumn<string>(
                name: "GroupID",
                table: "Group",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
