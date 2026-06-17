using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaSwap.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserVocabStats_UserId_SourceLanguage_TargetLanguage",
                table: "UserVocabStats",
                columns: new[] { "UserId", "SourceLanguage", "TargetLanguage" });

            migrationBuilder.CreateIndex(
                name: "IX_practice_sessions_UserId_SourceLanguage_TargetLanguage",
                table: "practice_sessions",
                columns: new[] { "UserId", "SourceLanguage", "TargetLanguage" });

            migrationBuilder.CreateIndex(
                name: "IX_attempts_SessionId_CreatedAt",
                table: "attempts",
                columns: new[] { "SessionId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserVocabStats_UserId_SourceLanguage_TargetLanguage",
                table: "UserVocabStats");

            migrationBuilder.DropIndex(
                name: "IX_practice_sessions_UserId_SourceLanguage_TargetLanguage",
                table: "practice_sessions");

            migrationBuilder.DropIndex(
                name: "IX_attempts_SessionId_CreatedAt",
                table: "attempts");
        }
    }
}
