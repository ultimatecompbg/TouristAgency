using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristAgency.Migrations
{
    /// <inheritdoc />
    public partial class DescrTrvlPckg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TravelPackages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TravelPackages");
        }
    }
}
