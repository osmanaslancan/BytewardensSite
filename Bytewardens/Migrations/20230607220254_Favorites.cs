using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bytewardens.Migrations
{
    public partial class Favorites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Favorites",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Favorites",
                table: "AspNetUsers");
        }
    }
}
