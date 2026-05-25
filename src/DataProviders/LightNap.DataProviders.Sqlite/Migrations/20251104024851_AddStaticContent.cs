using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightNap.DataProviders.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddStaticContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaticContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusChangedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StatusChangedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    EditorRoles = table.Column<string>(type: "TEXT", nullable: true),
                    ReadAccess = table.Column<int>(type: "INTEGER", nullable: false),
                    ReaderRoles = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaticContents_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaticContents_AspNetUsers_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaticContents_AspNetUsers_StatusChangedByUserId",
                        column: x => x.StatusChangedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StaticContentLanguages",
                columns: table => new
                {
                    StaticContentId = table.Column<int>(type: "INTEGER", nullable: false),
                    LanguageCode = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Format = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedUserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticContentLanguages", x => new { x.StaticContentId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_StaticContentLanguages_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaticContentLanguages_AspNetUsers_LastModifiedUserId",
                        column: x => x.LastModifiedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaticContentLanguages_StaticContents_StaticContentId",
                        column: x => x.StaticContentId,
                        principalTable: "StaticContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaticContentLanguages_CreatedByUserId",
                table: "StaticContentLanguages",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StaticContentLanguages_LanguageCode",
                table: "StaticContentLanguages",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_StaticContentLanguages_LastModifiedUserId",
                table: "StaticContentLanguages",
                column: "LastModifiedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StaticContents_CreatedByUserId",
                table: "StaticContents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StaticContents_Key",
                table: "StaticContents",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaticContents_LastModifiedByUserId",
                table: "StaticContents",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StaticContents_StatusChangedByUserId",
                table: "StaticContents",
                column: "StatusChangedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaticContentLanguages");

            migrationBuilder.DropTable(
                name: "StaticContents");
        }
    }
}
