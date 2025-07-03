using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistrySystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addBranches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_Owners_OwnerId",
                table: "Farms");

            migrationBuilder.DropIndex(
                name: "IX_FarmBoundaries_FarmId",
                table: "FarmBoundaries");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Farms");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Farms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FarmBranchId",
                table: "FarmDocuments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FarmBranch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Area = table.Column<double>(type: "float", nullable: false),
                    BoundariesId = table.Column<int>(type: "int", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    FarmId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmBranch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FarmBranch_FarmBoundaries_BoundariesId",
                        column: x => x.BoundariesId,
                        principalTable: "FarmBoundaries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FarmBranch_Farms_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FarmBranch_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FarmDocuments_FarmBranchId",
                table: "FarmDocuments",
                column: "FarmBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmBoundaries_FarmId",
                table: "FarmBoundaries",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmBranch_BoundariesId",
                table: "FarmBranch",
                column: "BoundariesId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmBranch_FarmId",
                table: "FarmBranch",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmBranch_OwnerId",
                table: "FarmBranch",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmDocuments_FarmBranch_FarmBranchId",
                table: "FarmDocuments",
                column: "FarmBranchId",
                principalTable: "FarmBranch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_Owners_OwnerId",
                table: "Farms",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmDocuments_FarmBranch_FarmBranchId",
                table: "FarmDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Farms_Owners_OwnerId",
                table: "Farms");

            migrationBuilder.DropTable(
                name: "FarmBranch");

            migrationBuilder.DropIndex(
                name: "IX_FarmDocuments_FarmBranchId",
                table: "FarmDocuments");

            migrationBuilder.DropIndex(
                name: "IX_FarmBoundaries_FarmId",
                table: "FarmBoundaries");

            migrationBuilder.DropColumn(
                name: "FarmBranchId",
                table: "FarmDocuments");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Farms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Area",
                table: "Farms",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_FarmBoundaries_FarmId",
                table: "FarmBoundaries",
                column: "FarmId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_Owners_OwnerId",
                table: "Farms",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
