using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inventory_AddSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "StockMovmentStatus",
                table: "StockMovements",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldDefaultValue: (byte)1);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Inventory",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Inventory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Inventory",
                type: "bit",
                nullable: false,
                defaultValueSql: "((0))");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_DeletedByUserId",
                table: "Inventory",
                column: "DeletedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Users_DeletedByUserId",
                table: "Inventory",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Users_DeletedByUserId",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_DeletedByUserId",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Inventory");

            migrationBuilder.AlterColumn<byte>(
                name: "StockMovmentStatus",
                table: "StockMovements",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)1,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
