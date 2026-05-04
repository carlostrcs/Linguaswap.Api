using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaSwap.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentWordIdToPracticeSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentWordId",
                table: "practice_sessions",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentWordId",
                table: "practice_sessions");
        }
    }
}
