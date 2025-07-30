using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristAgency.Migrations
{
    /// <inheritdoc />
    public partial class AddTravelPackageImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TravelPackageImage_TravelPackages_TravelPackageId",
                table: "TravelPackageImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TravelPackageImage",
                table: "TravelPackageImage");

            migrationBuilder.RenameTable(
                name: "TravelPackageImage",
                newName: "TravelPackageImages");

            migrationBuilder.RenameIndex(
                name: "IX_TravelPackageImage_TravelPackageId",
                table: "TravelPackageImages",
                newName: "IX_TravelPackageImages_TravelPackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TravelPackageImages",
                table: "TravelPackageImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TravelPackageImages_TravelPackages_TravelPackageId",
                table: "TravelPackageImages",
                column: "TravelPackageId",
                principalTable: "TravelPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TravelPackageImages_TravelPackages_TravelPackageId",
                table: "TravelPackageImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TravelPackageImages",
                table: "TravelPackageImages");

            migrationBuilder.RenameTable(
                name: "TravelPackageImages",
                newName: "TravelPackageImage");

            migrationBuilder.RenameIndex(
                name: "IX_TravelPackageImages_TravelPackageId",
                table: "TravelPackageImage",
                newName: "IX_TravelPackageImage_TravelPackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TravelPackageImage",
                table: "TravelPackageImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TravelPackageImage_TravelPackages_TravelPackageId",
                table: "TravelPackageImage",
                column: "TravelPackageId",
                principalTable: "TravelPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
