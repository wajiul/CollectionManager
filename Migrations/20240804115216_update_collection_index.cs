using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectionManager.Migrations
{
    /// <inheritdoc />
    public partial class update_collection_index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_collections_Name_Category",
                table: "collections");

            migrationBuilder.DropIndex(
                name: "IX_collections_UserId",
                table: "collections");

            migrationBuilder.CreateIndex(
                name: "IX_collections_UserId_Name_Category",
                table: "collections",
                columns: new[] { "UserId", "Name", "Category" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_collections_UserId_Name_Category",
                table: "collections");

            migrationBuilder.CreateIndex(
                name: "IX_collections_Name_Category",
                table: "collections",
                columns: new[] { "Name", "Category" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_collections_UserId",
                table: "collections",
                column: "UserId");
        }
    }
}
