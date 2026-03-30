using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToSharedEntityBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "UserSessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StockTransfers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "StockTransfers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StockTransfers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StockMovements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StockMovements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SalesOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "SalesOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SalesOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SalesOrderReservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "SalesOrderReservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SalesOrderReservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SalesOrderItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "SalesOrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SalesOrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PurchaseOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "PurchaseOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PurchaseOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PurchaseOrderItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "PurchaseOrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PurchaseOrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ProductImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "ProductImages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Image",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Image",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Image",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ConfirmEmailTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "ConfirmEmailTokens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ConfirmEmailTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AuditLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "AuditLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AuditLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AlertTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "AlertTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AlertTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AlertRules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "AlertRules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AlertRules",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SalesOrderReservations");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "SalesOrderReservations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SalesOrderReservations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SalesOrderItems");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "SalesOrderItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SalesOrderItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ConfirmEmailTokens");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ConfirmEmailTokens");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ConfirmEmailTokens");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AlertTypes");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "AlertTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AlertTypes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AlertRules");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "AlertRules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AlertRules");
        }
    }
}
