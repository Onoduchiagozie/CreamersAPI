using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdonisAPI.Migrations
{
    public partial class InitialCreate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_GymBro_GymBroId",
                table: "Favorites");

            migrationBuilder.RenameColumn(
                name: "Gymbro",
                table: "GymBro",
                newName: "FullName");

            migrationBuilder.AlterColumn<string>(
                name: "GymBroId",
                table: "Favorites",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "GymBroId",
                table: "Favorites",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "GymBroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_GymBro_GymBroId",
                table: "Favorites",
                column: "GymBroId",
                principalTable: "GymBro",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_GymBro_UserId",
                table: "Favorites",
                column: "GymBroId",
                principalTable: "GymBro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_GymBro_GymBroId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_GymBro_UserId",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "GymBro",
                newName: "Gymbro");

            migrationBuilder.AlterColumn<int>(
                name: "GymBroId",
                table: "Favorites",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "GymBroId",
                table: "Favorites",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_GymBro_GymBroId",
                table: "Favorites",
                column: "GymBroId",
                principalTable: "GymBro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
