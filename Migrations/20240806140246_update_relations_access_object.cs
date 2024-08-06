using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectionManager.Migrations
{
    /// <inheritdoc />
    public partial class update_relations_access_object : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_items_ItemId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_customFields_collections_CollectionId",
                table: "customFields");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_items_ItemId",
                table: "likes");

            migrationBuilder.CreateTable(
                name: "ItemModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomFieldValueModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFieldValueModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomFieldValueModel_ItemModel_ItemModelId",
                        column: x => x.ItemModelId,
                        principalTable: "ItemModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomFieldValueModel_ItemModelId",
                table: "CustomFieldValueModel",
                column: "ItemModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_items_ItemId",
                table: "comments",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customFields_collections_CollectionId",
                table: "customFields",
                column: "CollectionId",
                principalTable: "collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_likes_items_ItemId",
                table: "likes",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_items_ItemId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_customFields_collections_CollectionId",
                table: "customFields");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_items_ItemId",
                table: "likes");

            migrationBuilder.DropTable(
                name: "CustomFieldValueModel");

            migrationBuilder.DropTable(
                name: "ItemModel");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_items_ItemId",
                table: "comments",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_customFields_collections_CollectionId",
                table: "customFields",
                column: "CollectionId",
                principalTable: "collections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_likes_items_ItemId",
                table: "likes",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id");
        }
    }
}
