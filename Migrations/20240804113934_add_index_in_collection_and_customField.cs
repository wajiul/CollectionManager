using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectionManager.Migrations
{
    /// <inheritdoc />
    public partial class add_index_in_collection_and_customField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FieldType",
                table: "customFields",
                newName: "Type");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "customFields",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "collections",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "collections",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_customFields_Name_Type_CollectionId",
                table: "customFields",
                columns: new[] { "Name", "Type", "CollectionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_collections_Name_Category",
                table: "collections",
                columns: new[] { "Name", "Category" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_customFields_Name_Type_CollectionId",
                table: "customFields");

            migrationBuilder.DropIndex(
                name: "IX_collections_Name_Category",
                table: "collections");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "customFields",
                newName: "FieldType");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "customFields",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "collections",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "collections",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
