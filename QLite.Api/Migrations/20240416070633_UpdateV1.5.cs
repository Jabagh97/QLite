using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLiteDataApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateV15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "TicketState",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "TicketState");
        }
    }
}
