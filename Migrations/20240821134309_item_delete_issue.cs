using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectionManager.Migrations
{
    /// <inheritdoc />
    public partial class item_delete_issue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_items_ItemId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_items_ItemId",
                table: "likes");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_items_ItemId",
                table: "comments",
                column: "ItemId",
                principalTable: "items",
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
                name: "FK_likes_items_ItemId",
                table: "likes");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_items_ItemId",
                table: "comments",
                column: "ItemId",
                principalTable: "items",
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
