using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristAgency.Migrations
{
    /// <inheritdoc />
    public partial class migration3436 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "TravelPackageImages",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "TravelPackageImages",
                newName: "FileName");
        }
    }
}
