using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facebook_MKT.Data.Migrations
{
    /// <inheritdoc />
    public partial class changKeyTablePage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pages",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_PageID",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PageIdKey",
                table: "Pages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pages",
                table: "Pages",
                column: "PageID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pages",
                table: "Pages");

            migrationBuilder.AddColumn<int>(
                name: "PageIdKey",
                table: "Pages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pages",
                table: "Pages",
                column: "PageIdKey");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PageID",
                table: "Pages",
                column: "PageID",
                unique: true);
        }
    }
}
