using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistrySystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BarcodeImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "BarcodeImage",
                table: "Farms",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarcodeImage",
                table: "Farms");
        }
    }
}
