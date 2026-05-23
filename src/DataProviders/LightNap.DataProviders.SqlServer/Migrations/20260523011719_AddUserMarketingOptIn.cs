using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightNap.DataProviders.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddUserMarketingOptIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MarketingOptIn",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "MarketingOptInAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketingOptIn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MarketingOptInAt",
                table: "AspNetUsers");
        }
    }
}
