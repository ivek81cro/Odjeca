using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odjeca.Migrations
{
    public partial class RemoveRequiredForBrandsImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Image",
                table: "Brands",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Image",
                table: "Brands",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldNullable: true);
        }
    }
}
