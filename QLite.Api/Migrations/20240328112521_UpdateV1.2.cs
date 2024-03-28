using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLiteDataApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateV12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "iTicketPool_Ticket",
                table: "Ticket",
                column: "TicketPool");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_TicketPool",
                table: "Ticket",
                column: "TicketPool",
                principalTable: "TicketPool",
                principalColumn: "Oid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_TicketPool",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "iTicketPool_Ticket",
                table: "Ticket");
        }
    }
}
