using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaSwap.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLibraryPublicFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "libraries",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "libraries");
        }
    }
}
