using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdonisAPI.Migrations
{
    public partial class RegisterUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Goal",
                table: "GymBro",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Goal",
                table: "GymBro");
        }
    }
}
