using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLiteDataApi.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DesignImage",
                table: "Design",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "iAccount_DesignTarget",
                table: "DesignTarget",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "iBranch_DesignTarget",
                table: "DesignTarget",
                column: "Branch");

            migrationBuilder.CreateIndex(
                name: "iKiosk_DesignTarget",
                table: "DesignTarget",
                column: "Kiosk");

            migrationBuilder.CreateIndex(
                name: "iAccount_Design",
                table: "Design",
                column: "Account");

            migrationBuilder.AddForeignKey(
                name: "FK_Design_Account",
                table: "Design",
                column: "Account",
                principalTable: "Account",
                principalColumn: "Oid");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Design_Account",
                table: "Design");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignTarget_Account",
                table: "DesignTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignTarget_Branch",
                table: "DesignTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignTarget_Kiosk",
                table: "DesignTarget");

            migrationBuilder.DropIndex(
                name: "iAccount_DesignTarget",
                table: "DesignTarget");

            migrationBuilder.DropIndex(
                name: "iBranch_DesignTarget",
                table: "DesignTarget");

            migrationBuilder.DropIndex(
                name: "iKiosk_DesignTarget",
                table: "DesignTarget");

            migrationBuilder.DropIndex(
                name: "iAccount_Design",
                table: "Design");

            migrationBuilder.DropColumn(
                name: "DesignImage",
                table: "Design");
        }
    }
}
