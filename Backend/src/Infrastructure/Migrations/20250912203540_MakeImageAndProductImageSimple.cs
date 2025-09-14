using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeImageAndProductImageSimple : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Users_DeletedByUserId",
                table: "Image");

            migrationBuilder.DropForeignKey(
                name: "FK_Image_Users_UpdatedByUserId",
                table: "Image");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_DeletedByUser",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Image_ImageId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Users_UpdatedByUserId",
                table: "ProductImages");

            //migrationBuilder.DropIndex(
            //    name: "IX_ProductImages_DeletedByUserId",
            //    table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_UpdatedByUserId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_Image_DeletedByUserId",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_UpdatedByUserId",
                table: "Image");

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
                name: "UpdatedAt",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
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
                name: "UpdatedAt",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Image");

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                table: "ProductImages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "SizeInBytes",
                table: "Image",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Image_ImageId",
                table: "ProductImages",
                column: "ImageId",
                principalTable: "Image",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Image_ImageId",
                table: "ProductImages");

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                table: "ProductImages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ProductImages",
                type: "datetime",
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
                name: "UpdatedAt",
                table: "ProductImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "ProductImages",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SizeInBytes",
                table: "Image",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Image",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Image",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Image",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Image",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Image",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_DeletedByUserId",
                table: "ProductImages",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_UpdatedByUserId",
                table: "ProductImages",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_DeletedByUserId",
                table: "Image",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_UpdatedByUserId",
                table: "Image",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Users_DeletedByUserId",
                table: "Image",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Users_UpdatedByUserId",
                table: "Image",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_DeletedByUser",
                table: "ProductImages",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Image_ImageId",
                table: "ProductImages",
                column: "ImageId",
                principalTable: "Image",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Users_UpdatedByUserId",
                table: "ProductImages",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
