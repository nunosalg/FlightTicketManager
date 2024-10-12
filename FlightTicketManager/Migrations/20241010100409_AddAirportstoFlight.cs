using Microsoft.EntityFrameworkCore.Migrations;

namespace FlightTicketManager.Migrations
{
    public partial class AddAirportstoFlight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DestinationAirport",
                table: "TicketsHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginAirport",
                table: "TicketsHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationAirport",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginAirport",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationAirport",
                table: "TicketsHistory");

            migrationBuilder.DropColumn(
                name: "OriginAirport",
                table: "TicketsHistory");

            migrationBuilder.DropColumn(
                name: "DestinationAirport",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "OriginAirport",
                table: "Flights");
        }
    }
}
