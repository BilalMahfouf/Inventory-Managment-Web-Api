using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UnitOfMeasures_AddUpdatedAtandByUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UnitOfMeasures",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "UnitOfMeasures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_UpdatedByUserId",
                table: "UnitOfMeasures",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitOfMeasures_Users_UpdatedByUserId",
                table: "UnitOfMeasures",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitOfMeasures_Users_UpdatedByUserId",
                table: "UnitOfMeasures");

            migrationBuilder.DropIndex(
                name: "IX_UnitOfMeasures_UpdatedByUserId",
                table: "UnitOfMeasures");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UnitOfMeasures");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "UnitOfMeasures");
        }
    }
}
