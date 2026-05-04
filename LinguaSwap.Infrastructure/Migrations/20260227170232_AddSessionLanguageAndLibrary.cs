using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaSwap.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionLanguageAndLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LibraryId",
                table: "practice_sessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceLanguage",
                table: "practice_sessions",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TargetLanguage",
                table: "practice_sessions",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_practice_sessions_LibraryId",
                table: "practice_sessions",
                column: "LibraryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_practice_sessions_LibraryId",
                table: "practice_sessions");

            migrationBuilder.DropColumn(
                name: "LibraryId",
                table: "practice_sessions");

            migrationBuilder.DropColumn(
                name: "SourceLanguage",
                table: "practice_sessions");

            migrationBuilder.DropColumn(
                name: "TargetLanguage",
                table: "practice_sessions");
        }
    }
}
