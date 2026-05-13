using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaFashion.API.Migrations
{
    /// <inheritdoc />
    public partial class M_ProductRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "ProductRatings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProductRatings_OrderId",
                table: "ProductRatings",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductRatings_Orders_OrderId",
                table: "ProductRatings",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductRatings_Orders_OrderId",
                table: "ProductRatings");

            migrationBuilder.DropIndex(
                name: "IX_ProductRatings_OrderId",
                table: "ProductRatings");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ProductRatings");
        }
    }
}
