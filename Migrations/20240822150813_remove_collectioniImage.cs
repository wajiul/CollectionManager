using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectionManager.Migrations
{
    /// <inheritdoc />
    public partial class remove_collectioniImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "collections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "collections",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
