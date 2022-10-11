using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeleBotOrders.Migrations
{
    public partial class addfieldCountInDish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Dishes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Dishes");
        }
    }
}
