using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLiteDataApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateV11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Desk_Pano",
                table: "Desk");

            migrationBuilder.RenameColumn(
                name: "Pano",
                table: "Desk",
                newName: "Kiosk");

            migrationBuilder.RenameIndex(
                name: "iPano_Desk",
                table: "Desk",
                newName: "iKiosk_Desk");

            migrationBuilder.AddForeignKey(
                name: "FK_Desk_Kiosk",
                table: "Desk",
                column: "Kiosk",
                principalTable: "Kiosk",
                principalColumn: "Oid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Desk_Kiosk",
                table: "Desk");

            migrationBuilder.RenameColumn(
                name: "Kiosk",
                table: "Desk",
                newName: "Pano");

            migrationBuilder.RenameIndex(
                name: "iKiosk_Desk",
                table: "Desk",
                newName: "iPano_Desk");

            migrationBuilder.AddForeignKey(
                name: "FK_Desk_Pano",
                table: "Desk",
                column: "Pano",
                principalTable: "Kiosk",
                principalColumn: "Oid");
        }
    }
}
