using Microsoft.EntityFrameworkCore.Migrations;

namespace FlightTicketManager.Migrations
{
    public partial class EditFlight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Flights");

            migrationBuilder.AddColumn<int>(
                name: "DestinationId",
                table: "Flights",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginId",
                table: "Flights",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DestinationId",
                table: "Flights",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_OriginId",
                table: "Flights",
                column: "OriginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Cities_DestinationId",
                table: "Flights",
                column: "DestinationId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Cities_OriginId",
                table: "Flights",
                column: "OriginId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Cities_DestinationId",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Cities_OriginId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_DestinationId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_OriginId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "OriginId",
                table: "Flights");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
