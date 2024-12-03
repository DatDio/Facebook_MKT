using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facebook_MKT.Data.Migrations
{
    /// <inheritdoc />
    public partial class addTableGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_FolderGroup_FolderGroupFolderIdKey",
                table: "Group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group",
                table: "Group");

            migrationBuilder.DropIndex(
                name: "IX_Group_GroupID",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "GroupIdKey",
                table: "Group");

            migrationBuilder.RenameTable(
                name: "Group",
                newName: "Groups");

            migrationBuilder.RenameIndex(
                name: "IX_Group_FolderGroupFolderIdKey",
                table: "Groups",
                newName: "IX_Groups_FolderGroupFolderIdKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_FolderGroup_FolderGroupFolderIdKey",
                table: "Groups",
                column: "FolderGroupFolderIdKey",
                principalTable: "FolderGroup",
                principalColumn: "FolderIdKey",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_FolderGroup_FolderGroupFolderIdKey",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "Group");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_FolderGroupFolderIdKey",
                table: "Group",
                newName: "IX_Group_FolderGroupFolderIdKey");

            migrationBuilder.AddColumn<int>(
                name: "GroupIdKey",
                table: "Group",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group",
                table: "Group",
                column: "GroupIdKey");

            migrationBuilder.CreateIndex(
                name: "IX_Group_GroupID",
                table: "Group",
                column: "GroupID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Group_FolderGroup_FolderGroupFolderIdKey",
                table: "Group",
                column: "FolderGroupFolderIdKey",
                principalTable: "FolderGroup",
                principalColumn: "FolderIdKey",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
