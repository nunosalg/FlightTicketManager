using Microsoft.EntityFrameworkCore.Migrations;

namespace FlightTicketManager.Migrations
{
    public partial class JsonConverter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableSeatsJson",
                table: "Flights",
                newName: "AvailableSeats");

            migrationBuilder.RenameColumn(
                name: "SeatsJson",
                table: "Aircrafts",
                newName: "Seats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableSeats",
                table: "Flights",
                newName: "AvailableSeatsJson");

            migrationBuilder.RenameColumn(
                name: "Seats",
                table: "Aircrafts",
                newName: "SeatsJson");
        }
    }
}
