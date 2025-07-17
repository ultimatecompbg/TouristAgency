using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristAgency.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToDestinationAndPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetRoles");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Destinations",
                newName: "ImagePath");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "TravelPackages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "TravelPackages");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Destinations",
                newName: "ImageUrl");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetRoles",
                type: "TEXT",
                maxLength: 21,
                nullable: false,
                defaultValue: "");
        }
    }
}
