using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClutchBids.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFeaturedToCarListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "CarListings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "CarListings");
        }
    }
}
