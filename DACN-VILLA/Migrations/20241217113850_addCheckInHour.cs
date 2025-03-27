using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACN_VILLA.Migrations
{
    /// <inheritdoc />
    public partial class addCheckInHour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckInHour",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInHour",
                table: "Bookings");
        }
    }
}
