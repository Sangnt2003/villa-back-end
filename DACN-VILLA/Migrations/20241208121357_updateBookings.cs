using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACN_VILLA.Migrations
{
    /// <inheritdoc />
    public partial class updateBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BookingProcesses");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "BookingProcesses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "BookingProcesses");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BookingProcesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
