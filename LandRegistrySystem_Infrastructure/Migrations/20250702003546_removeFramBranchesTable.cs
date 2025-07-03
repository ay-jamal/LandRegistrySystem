using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistrySystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeFramBranchesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmDocuments_FarmBranches_FarmBranchId",
                table: "FarmDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Farms_Owners_OwnerId",
                table: "Farms");

            migrationBuilder.DropTable(
                name: "FarmBranches");

            migrationBuilder.DropIndex(
                name: "IX_FarmBoundaries_FarmId",
                table: "FarmBoundaries");

            migrationBuilder.RenameColumn(
                name: "FarmBranchId",
                table: "FarmDocuments",
                newName: "FarmId");

            migrationBuilder.RenameIndex(
                name: "IX_FarmDocuments_FarmBranchId",
                table: "FarmDocuments",
                newName: "IX_FarmDocuments_FarmId");

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
                name: "FK_FarmDocuments_Farms_FarmId",
                table: "FarmDocuments",
                column: "FarmId",
                principalTable: "Farms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_Owners_OwnerId",
                table: "Farms",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmDocuments_Farms_FarmId",
                table: "FarmDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Farms_Owners_OwnerId",
                table: "Farms");

            migrationBuilder.DropIndex(
                name: "IX_FarmBoundaries_FarmId",
                table: "FarmBoundaries");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Farms");

            migrationBuilder.RenameColumn(
                name: "FarmId",
                table: "FarmDocuments",
                newName: "FarmBranchId");

            migrationBuilder.RenameIndex(
                name: "IX_FarmDocuments_FarmId",
                table: "FarmDocuments",
                newName: "IX_FarmDocuments_FarmBranchId");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Farms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "FarmBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoundariesId = table.Column<int>(type: "int", nullable: true),
                    FarmId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FarmBranches_FarmBoundaries_BoundariesId",
                        column: x => x.BoundariesId,
                        principalTable: "FarmBoundaries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FarmBranches_Farms_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FarmBranches_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FarmBoundaries_FarmId",
                table: "FarmBoundaries",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmBranches_BoundariesId",
                table: "FarmBranches",
                column: "BoundariesId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmBranches_FarmId",
                table: "FarmBranches",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmBranches_OwnerId",
                table: "FarmBranches",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmDocuments_FarmBranches_FarmBranchId",
                table: "FarmDocuments",
                column: "FarmBranchId",
                principalTable: "FarmBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_Owners_OwnerId",
                table: "Farms",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id");
        }
    }
}
