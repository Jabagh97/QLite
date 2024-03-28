using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLiteDataApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "iAccount_DeskTransferableService",
                table: "DeskTransferableServices",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iAccount_DeskCreatableServices",
                table: "DeskCreatableServices",
                column: "Account");

            migrationBuilder.AddForeignKey(
                name: "FK_DeskCreatableServices_Account",
                table: "DeskCreatableServices",
                column: "Account",
                principalTable: "Account",
                principalColumn: "Oid");

            migrationBuilder.AddForeignKey(
                name: "FK_DeskTransferableServices_Account",
                table: "DeskTransferableServices",
                column: "Account",
                principalTable: "Account",
                principalColumn: "Oid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeskCreatableServices_Account",
                table: "DeskCreatableServices");

            migrationBuilder.DropForeignKey(
                name: "FK_DeskTransferableServices_Account",
                table: "DeskTransferableServices");

            migrationBuilder.DropIndex(
                name: "iAccount_DeskTransferableService",
                table: "DeskTransferableServices");

            migrationBuilder.DropIndex(
                name: "iAccount_DeskCreatableServices",
                table: "DeskCreatableServices");
        }
    }
}
