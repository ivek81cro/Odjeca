using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odjeca.Migrations
{
    public partial class AddArrivaldateToStoreItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalDate",
                table: "StoreItem",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalDate",
                table: "StoreItem");
        }
    }
}
