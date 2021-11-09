using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowStone.Repository.Migrations
{
    public partial class RequestState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestState",
                table: "CustomerRequests",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestState",
                table: "CustomerRequests");
        }
    }
}
