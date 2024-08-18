using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectionManager.Migrations
{
    /// <inheritdoc />
    public partial class item_many_to_many_tag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemTag_items_ItemsId",
                table: "ItemTag");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemTag_tags_TagsId",
                table: "ItemTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemTag",
                table: "ItemTag");

            migrationBuilder.RenameTable(
                name: "ItemTag",
                newName: "ItemTags");

            migrationBuilder.RenameIndex(
                name: "IX_ItemTag_TagsId",
                table: "ItemTags",
                newName: "IX_ItemTags_TagsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemTags",
                table: "ItemTags",
                columns: new[] { "ItemsId", "TagsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTags_items_ItemsId",
                table: "ItemTags",
                column: "ItemsId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTags_tags_TagsId",
                table: "ItemTags",
                column: "TagsId",
                principalTable: "tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemTags_items_ItemsId",
                table: "ItemTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemTags_tags_TagsId",
                table: "ItemTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemTags",
                table: "ItemTags");

            migrationBuilder.RenameTable(
                name: "ItemTags",
                newName: "ItemTag");

            migrationBuilder.RenameIndex(
                name: "IX_ItemTags_TagsId",
                table: "ItemTag",
                newName: "IX_ItemTag_TagsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemTag",
                table: "ItemTag",
                columns: new[] { "ItemsId", "TagsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTag_items_ItemsId",
                table: "ItemTag",
                column: "ItemsId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTag_tags_TagsId",
                table: "ItemTag",
                column: "TagsId",
                principalTable: "tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
