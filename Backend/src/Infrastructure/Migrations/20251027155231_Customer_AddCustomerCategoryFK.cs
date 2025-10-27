using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Customer_AddCustomerCategoryFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerCategoryId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerCategoryId",
                table: "Customers",
                column: "CustomerCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CustomerCategories_CustomerCategoryId",
                table: "Customers",
                column: "CustomerCategoryId",
                principalTable: "CustomerCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CustomerCategories_CustomerCategoryId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CustomerCategoryId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerCategoryId",
                table: "Customers");
        }
    }
}
