using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACN_VILLA.Migrations
{
    /// <inheritdoc />
    public partial class updateVillas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Villas_Discounts_DiscountId",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "Villas");

            migrationBuilder.AlterColumn<Guid>(
                name: "DiscountId",
                table: "Villas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Villas_Discounts_DiscountId",
                table: "Villas",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Villas_Discounts_DiscountId",
                table: "Villas");

            migrationBuilder.AlterColumn<Guid>(
                name: "DiscountId",
                table: "Villas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "Villas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_Villas_Discounts_DiscountId",
                table: "Villas",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
