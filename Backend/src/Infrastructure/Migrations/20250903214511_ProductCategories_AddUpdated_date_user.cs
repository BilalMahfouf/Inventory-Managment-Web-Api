using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductCategories_AddUpdated_date_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "ProductCategories",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "ProductCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_UpdatedByUserId",
                table: "ProductCategories",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Users_UpdatedByUserId",
                table: "ProductCategories",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Users_UpdatedByUserId",
                table: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_UpdatedByUserId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ProductCategories");
        }
    }
}
