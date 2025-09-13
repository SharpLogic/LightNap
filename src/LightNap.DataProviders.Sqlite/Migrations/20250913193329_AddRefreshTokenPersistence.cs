using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightNap.DataProviders.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPresistent",
                table: "RefreshTokens",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPresistent",
                table: "RefreshTokens");
        }
    }
}
