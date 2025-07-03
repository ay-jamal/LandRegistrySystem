using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistrySystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addFramBranchesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmBranch_FarmBoundaries_BoundariesId",
                table: "FarmBranch");

            migrationBuilder.DropForeignKey(
                name: "FK_FarmBranch_Farms_FarmId",
                table: "FarmBranch");

            migrationBuilder.DropForeignKey(
                name: "FK_FarmBranch_Owners_OwnerId",
                table: "FarmBranch");

            migrationBuilder.DropForeignKey(
                name: "FK_FarmDocuments_FarmBranch_FarmBranchId",
                table: "FarmDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FarmBranch",
                table: "FarmBranch");

            migrationBuilder.RenameTable(
                name: "FarmBranch",
                newName: "FarmBranches");

            migrationBuilder.RenameIndex(
                name: "IX_FarmBranch_OwnerId",
                table: "FarmBranches",
                newName: "IX_FarmBranches_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_FarmBranch_FarmId",
                table: "FarmBranches",
                newName: "IX_FarmBranches_FarmId");

            migrationBuilder.RenameIndex(
                name: "IX_FarmBranch_BoundariesId",
                table: "FarmBranches",
                newName: "IX_FarmBranches_BoundariesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FarmBranches",
                table: "FarmBranches",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmBranches_FarmBoundaries_BoundariesId",
                table: "FarmBranches",
                column: "BoundariesId",
                principalTable: "FarmBoundaries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmBranches_Farms_FarmId",
                table: "FarmBranches",
                column: "FarmId",
                principalTable: "Farms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FarmBranches_Owners_OwnerId",
                table: "FarmBranches",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FarmDocuments_FarmBranches_FarmBranchId",
                table: "FarmDocuments",
                column: "FarmBranchId",
                principalTable: "FarmBranches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmBranches_FarmBoundaries_BoundariesId",
                table: "FarmBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_FarmBranches_Farms_FarmId",
                table: "FarmBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_FarmBranches_Owners_OwnerId",
                table: "FarmBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_FarmDocuments_FarmBranches_FarmBranchId",
                table: "FarmDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FarmBranches",
                table: "FarmBranches");

            migrationBuilder.RenameTable(
                name: "FarmBranches",
                newName: "FarmBranch");

            migrationBuilder.RenameIndex(
                name: "IX_FarmBranches_OwnerId",
                table: "FarmBranch",
                newName: "IX_FarmBranch_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_FarmBranches_FarmId",
                table: "FarmBranch",
                newName: "IX_FarmBranch_FarmId");

            migrationBuilder.RenameIndex(
                name: "IX_FarmBranches_BoundariesId",
                table: "FarmBranch",
                newName: "IX_FarmBranch_BoundariesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FarmBranch",
                table: "FarmBranch",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmBranch_FarmBoundaries_BoundariesId",
                table: "FarmBranch",
                column: "BoundariesId",
                principalTable: "FarmBoundaries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmBranch_Farms_FarmId",
                table: "FarmBranch",
                column: "FarmId",
                principalTable: "Farms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FarmBranch_Owners_OwnerId",
                table: "FarmBranch",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FarmDocuments_FarmBranch_FarmBranchId",
                table: "FarmDocuments",
                column: "FarmBranchId",
                principalTable: "FarmBranch",
                principalColumn: "Id");
        }
    }
}
