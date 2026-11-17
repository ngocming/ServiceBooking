using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceBooking.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAvailableToProviderService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "ProviderServices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "ProviderServices");
        }
    }
}
