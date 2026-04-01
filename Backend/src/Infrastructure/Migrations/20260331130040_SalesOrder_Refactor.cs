using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SalesOrder_Refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Customers",
                table: "SalesOrders");

            migrationBuilder.DropTable(
                name: "SalesOrderReservations");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "SalesOrders");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "SalesOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsWalkIn",
                table: "SalesOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "PaymentStatus",
                table: "SalesOrders",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)1);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "SalesOrders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "SalesOrders",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "SalesOrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "SalesOrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderItems_InventoryId",
                table: "SalesOrderItems",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrderItems_Inventory",
                table: "SalesOrderItems",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Customers",
                table: "SalesOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrderItems_Inventory",
                table: "SalesOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Customers",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrderItems_InventoryId",
                table: "SalesOrderItems");

            migrationBuilder.DropColumn(
                name: "IsWalkIn",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "SalesOrderItems");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "SalesOrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "SalesOrders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "SalesOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "SalesOrderReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<int>(type: "int", nullable: true),
                    InventoryId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusUpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StockMovemntId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderReservations_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesOrderReservations_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesOrderReservations_SalesOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesOrderReservations_StockMovements_StockMovemntId",
                        column: x => x.StockMovemntId,
                        principalTable: "StockMovements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesOrderReservations_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderReservations_CreatedByUserId",
                table: "SalesOrderReservations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderReservations_InventoryId",
                table: "SalesOrderReservations",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderReservations_OrderId",
                table: "SalesOrderReservations",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderReservations_ProductId",
                table: "SalesOrderReservations",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderReservations_StockMovemntId",
                table: "SalesOrderReservations",
                column: "StockMovemntId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Customers",
                table: "SalesOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
