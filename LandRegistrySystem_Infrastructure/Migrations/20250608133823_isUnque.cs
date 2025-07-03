using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistrySystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class isUnque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProjectNumber",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FarmNumber",
                table: "Farms",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CityNumber",
                table: "Citites",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectNumber",
                table: "Projects",
                column: "ProjectNumber",
                unique: true,
                filter: "[ProjectNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Farms_FarmNumber",
                table: "Farms",
                column: "FarmNumber",
                unique: true,
                filter: "[FarmNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Citites_CityNumber",
                table: "Citites",
                column: "CityNumber",
                unique: true,
                filter: "[CityNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectNumber",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Farms_FarmNumber",
                table: "Farms");

            migrationBuilder.DropIndex(
                name: "IX_Citites_CityNumber",
                table: "Citites");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectNumber",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FarmNumber",
                table: "Farms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CityNumber",
                table: "Citites",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
