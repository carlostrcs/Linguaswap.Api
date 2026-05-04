using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaSwap.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "libraries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_libraries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "practice_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_practice_sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vocab_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LibraryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocab_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vocab_items_libraries_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    WordId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserAnswer = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attempts_practice_sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "practice_sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vocab_terms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VocabItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Text = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocab_terms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vocab_terms_vocab_items_VocabItemId",
                        column: x => x.VocabItemId,
                        principalTable: "vocab_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attempts_SessionId",
                table: "attempts",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_attempts_WordId",
                table: "attempts",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_libraries_UserId",
                table: "libraries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_vocab_items_LibraryId",
                table: "vocab_items",
                column: "LibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_vocab_terms_VocabItemId_LanguageCode",
                table: "vocab_terms",
                columns: new[] { "VocabItemId", "LanguageCode" });

            migrationBuilder.CreateIndex(
                name: "IX_vocab_terms_VocabItemId_LanguageCode_Text",
                table: "vocab_terms",
                columns: new[] { "VocabItemId", "LanguageCode", "Text" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attempts");

            migrationBuilder.DropTable(
                name: "vocab_terms");

            migrationBuilder.DropTable(
                name: "practice_sessions");

            migrationBuilder.DropTable(
                name: "vocab_items");

            migrationBuilder.DropTable(
                name: "libraries");
        }
    }
}
