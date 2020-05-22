using Microsoft.EntityFrameworkCore.Migrations;

namespace Odjeca.Migrations
{
    public partial class ChangeNameOfForKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_StoreItem_MenuItemId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_MenuItemId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "MenuItemId",
                table: "OrderDetails");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_StoreItemId",
                table: "OrderDetails",
                column: "StoreItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_StoreItem_StoreItemId",
                table: "OrderDetails",
                column: "StoreItemId",
                principalTable: "StoreItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_StoreItem_StoreItemId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_StoreItemId",
                table: "OrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "MenuItemId",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_MenuItemId",
                table: "OrderDetails",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_StoreItem_MenuItemId",
                table: "OrderDetails",
                column: "MenuItemId",
                principalTable: "StoreItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
