using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeleBotOrders.Migrations
{
    public partial class changeTypeMenuId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Menus_MenuId1",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_MenuId1",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "MenuId1",
                table: "Dishes");

            migrationBuilder.AlterColumn<long>(
                name: "MenuId",
                table: "Dishes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_MenuId",
                table: "Dishes",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Menus_MenuId",
                table: "Dishes",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Menus_MenuId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_MenuId",
                table: "Dishes");

            migrationBuilder.AlterColumn<int>(
                name: "MenuId",
                table: "Dishes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "MenuId1",
                table: "Dishes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_MenuId1",
                table: "Dishes",
                column: "MenuId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Menus_MenuId1",
                table: "Dishes",
                column: "MenuId1",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
