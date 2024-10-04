using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlightTicketManager.Migrations
{
    public partial class ChangeTicketHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlightHistoryId",
                table: "TicketsHistory");

            migrationBuilder.AddColumn<DateTime>(
                name: "DepartureDateTime",
                table: "TicketsHistory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "TicketsHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlightNumber",
                table: "TicketsHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "TicketsHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketBuyer",
                table: "TicketsHistory",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartureDateTime",
                table: "TicketsHistory");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "TicketsHistory");

            migrationBuilder.DropColumn(
                name: "FlightNumber",
                table: "TicketsHistory");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "TicketsHistory");

            migrationBuilder.DropColumn(
                name: "TicketBuyer",
                table: "TicketsHistory");

            migrationBuilder.AddColumn<int>(
                name: "FlightHistoryId",
                table: "TicketsHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
