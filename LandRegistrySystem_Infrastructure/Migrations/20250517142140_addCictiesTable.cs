using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistrySystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addCictiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectNumber",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FarmNumber",
                table: "Farms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Citites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citites", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CityId",
                table: "Projects",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Citites_CityId",
                table: "Projects",
                column: "CityId",
                principalTable: "Citites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Citites_CityId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "Citites");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CityId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FarmNumber",
                table: "Farms");
        }
    }
}
