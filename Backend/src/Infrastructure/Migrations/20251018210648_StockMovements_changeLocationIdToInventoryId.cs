using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StockMovements_changeLocationIdToInventoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Locations",
                table: "StockMovements");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "StockMovements",
                newName: "InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_LocationId",
                table: "StockMovements",
                newName: "IX_StockMovements_InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Locations",
                table: "StockMovements",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Locations",
                table: "StockMovements");

            migrationBuilder.RenameColumn(
                name: "InventoryId",
                table: "StockMovements",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_InventoryId",
                table: "StockMovements",
                newName: "IX_StockMovements_LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Locations",
                table: "StockMovements",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }
    }
}
