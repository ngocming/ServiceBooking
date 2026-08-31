using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceBooking.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameServicesToProviderServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Services",
                newName: "ProviderServices");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ProviderId",
                table: "ProviderServices",
                newName: "IX_ProviderServices_ProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ProviderServices",
                newName: "Services");

            migrationBuilder.RenameIndex(
                name: "IX_ProviderServices_ProviderId",
                table: "Services",
                newName: "IX_Services_ProviderId");
        }
    }
}
