using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facebook_MKT.Data.Migrations
{
    /// <inheritdoc />
    public partial class addKeyGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_FolderGroup_FolderGroupFolderIdKey",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_FolderGroupFolderIdKey",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "FolderGroupFolderIdKey",
                table: "Groups");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_FolderIdKey",
                table: "Groups",
                column: "FolderIdKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_FolderGroup_FolderIdKey",
                table: "Groups",
                column: "FolderIdKey",
                principalTable: "FolderGroup",
                principalColumn: "FolderIdKey",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_FolderGroup_FolderIdKey",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_FolderIdKey",
                table: "Groups");

            migrationBuilder.AddColumn<int>(
                name: "FolderGroupFolderIdKey",
                table: "Groups",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_FolderGroupFolderIdKey",
                table: "Groups",
                column: "FolderGroupFolderIdKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_FolderGroup_FolderGroupFolderIdKey",
                table: "Groups",
                column: "FolderGroupFolderIdKey",
                principalTable: "FolderGroup",
                principalColumn: "FolderIdKey",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
