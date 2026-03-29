using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Customers_REmovePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditLimit",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PaymentTerms",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DefaultCreditLimit",
                table: "CustomerCategories");

            migrationBuilder.DropColumn(
                name: "DefaultPaymentTerms",
                table: "CustomerCategories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                       migrationBuilder.AddColumn<decimal>(
                name: "CreditLimit",
                table: "Customers",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 1000m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTerms",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "Net 30");

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultCreditLimit",
                table: "CustomerCategories",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 1000m);

            migrationBuilder.AddColumn<string>(
                name: "DefaultPaymentTerms",
                table: "CustomerCategories",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "Net 30");
        }
    }
}
