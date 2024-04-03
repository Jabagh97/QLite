using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLiteDataApi.Migrations
{
    /// <inheritdoc />
    public partial class Update14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfTickets",
                table: "MacroRule",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfTickets",
                table: "MacroRule");
        }
    }
}
