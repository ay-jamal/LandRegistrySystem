using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistrySystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRestrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_Projects_ProjectId",
                table: "Farms");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Citites_CityId",
                table: "Projects");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_Projects_ProjectId",
                table: "Farms",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Citites_CityId",
                table: "Projects",
                column: "CityId",
                principalTable: "Citites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_Projects_ProjectId",
                table: "Farms");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Citites_CityId",
                table: "Projects");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_Projects_ProjectId",
                table: "Farms",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Citites_CityId",
                table: "Projects",
                column: "CityId",
                principalTable: "Citites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
