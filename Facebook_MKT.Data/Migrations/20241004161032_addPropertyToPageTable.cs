using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facebook_MKT.Data.Migrations
{
    /// <inheritdoc />
    public partial class addPropertyToPageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PageFolderVideo",
                table: "Pages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ViewVideo1",
                table: "Pages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViewVideo2",
                table: "Pages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViewVideo3",
                table: "Pages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViewVideo4",
                table: "Pages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViewVideo5",
                table: "Pages",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageFolderVideo",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "ViewVideo1",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "ViewVideo2",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "ViewVideo3",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "ViewVideo4",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "ViewVideo5",
                table: "Pages");
        }
    }
}
