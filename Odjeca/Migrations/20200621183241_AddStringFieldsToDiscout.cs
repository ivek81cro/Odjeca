using Microsoft.EntityFrameworkCore.Migrations;

namespace Odjeca.Migrations
{
    public partial class AddStringFieldsToDiscout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Discount",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Discount",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Discount");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Discount");
        }
    }
}
