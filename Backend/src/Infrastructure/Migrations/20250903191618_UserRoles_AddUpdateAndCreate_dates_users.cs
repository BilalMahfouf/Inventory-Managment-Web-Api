using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserRoles_AddUpdateAndCreate_dates_users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserRoles",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "UserRoles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserRoles",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "UserRoles",
                type: "int",
                nullable: true);

           
            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_CreatedByUserId",
                table: "UserRoles",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UpdatedByUserId",
                table: "UserRoles",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_CreatedByUserId",
                table: "UserRoles",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UpdatedByUserId",
                table: "UserRoles",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_CreatedByUserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UpdatedByUserId",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_CreatedByUserId",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_UpdatedByUserId",
                table: "UserRoles");

           
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "UserRoles");

            
        }
    }
}
