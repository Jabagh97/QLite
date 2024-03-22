using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLiteDataApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DesignTarget_Account",
                table: "DesignTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignTarget_Branch",
                table: "DesignTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignTarget_Kiosk",
                table: "DesignTarget");

            migrationBuilder.AddForeignKey(
                name: "FK_DesignTarget_Account",
                table: "DesignTarget",
                column: "Account",
                principalTable: "Account",
                principalColumn: "Oid");

            migrationBuilder.AddForeignKey(
                name: "FK_DesignTarget_Branch",
                table: "DesignTarget",
                column: "Branch",
                principalTable: "Branch",
                principalColumn: "Oid");

            migrationBuilder.AddForeignKey(
                name: "FK_DesignTarget_Kiosk",
                table: "DesignTarget",
                column: "Kiosk",
                principalTable: "Kiosk",
                principalColumn: "Oid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DesignTarget_Account",
                table: "DesignTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignTarget_Branch",
                table: "DesignTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignTarget_Kiosk",
                table: "DesignTarget");

            migrationBuilder.AddForeignKey(
                name: "FK_DesignTarget_Account",
                table: "DesignTarget",
                column: "Design",
                principalTable: "Account",
                principalColumn: "Oid");

            migrationBuilder.AddForeignKey(
                name: "FK_DesignTarget_Branch",
                table: "DesignTarget",
                column: "Design",
                principalTable: "Branch",
                principalColumn: "Oid");

            migrationBuilder.AddForeignKey(
                name: "FK_DesignTarget_Kiosk",
                table: "DesignTarget",
                column: "Design",
                principalTable: "Kiosk",
                principalColumn: "Oid");
        }
    }
}
