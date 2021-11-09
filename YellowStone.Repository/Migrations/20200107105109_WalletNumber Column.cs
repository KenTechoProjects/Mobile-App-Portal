using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowStone.Repository.Migrations
{
    public partial class WalletNumberColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WalletNumber",
                table: "CustomerRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WalletNumber",
                table: "CustomerRequests");
        }
    }
}
