using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlightTicketManager.Migrations
{
    public partial class ChangeTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_PassengerId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_PassengerId",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "PassengerId",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PassengerBirthDate",
                table: "Tickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PassengerName",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TicketBuyerId",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketBuyerId",
                table: "Tickets",
                column: "TicketBuyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_TicketBuyerId",
                table: "Tickets",
                column: "TicketBuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_TicketBuyerId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketBuyerId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PassengerBirthDate",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PassengerName",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketBuyerId",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "PassengerId",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PassengerId",
                table: "Tickets",
                column: "PassengerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_PassengerId",
                table: "Tickets",
                column: "PassengerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
