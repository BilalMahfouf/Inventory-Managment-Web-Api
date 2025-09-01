using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixConfirmEmailTokensTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiredAtUtc",
                table: "ConfirmEmailToken",
                newName: "ExpiredAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "ConfirmEmailToken",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiredAt",
                table: "ConfirmEmailToken",
                newName: "ExpiredAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ConfirmEmailToken",
                newName: "CreatedAtUtc");
        }
    }
}
