using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistrySystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addUpdatedAtandBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmDocuments_FarmBranches_FarmBranchId",
                table: "FarmDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_FarmDocuments_Farms_FarmId",
                table: "FarmDocuments");

            migrationBuilder.DropIndex(
                name: "IX_FarmDocuments_FarmId",
                table: "FarmDocuments");

            migrationBuilder.DropColumn(
                name: "FarmId",
                table: "FarmDocuments");

            migrationBuilder.AlterColumn<int>(
                name: "FarmBranchId",
                table: "FarmDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "FarmBranches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "FarmBranches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FarmDocuments_FarmBranches_FarmBranchId",
                table: "FarmDocuments",
                column: "FarmBranchId",
                principalTable: "FarmBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmDocuments_FarmBranches_FarmBranchId",
                table: "FarmDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "FarmBranches");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "FarmBranches");

            migrationBuilder.AlterColumn<int>(
                name: "FarmBranchId",
                table: "FarmDocuments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FarmId",
                table: "FarmDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FarmDocuments_FarmId",
                table: "FarmDocuments",
                column: "FarmId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmDocuments_FarmBranches_FarmBranchId",
                table: "FarmDocuments",
                column: "FarmBranchId",
                principalTable: "FarmBranches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmDocuments_Farms_FarmId",
                table: "FarmDocuments",
                column: "FarmId",
                principalTable: "Farms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
