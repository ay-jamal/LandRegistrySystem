using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistrySystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarCode",
                table: "Farms");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Farms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BarCode",
                table: "Farms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Farms",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
