using Microsoft.EntityFrameworkCore.Migrations;

namespace FlightTicketManager.Migrations
{
    public partial class RemoveUserIdNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IdNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdNumber",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdNumber",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdNumber",
                table: "AspNetUsers",
                column: "IdNumber",
                unique: true,
                filter: "[IdNumber] IS NOT NULL");
        }
    }
}
