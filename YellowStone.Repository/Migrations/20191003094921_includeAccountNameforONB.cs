using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowStone.Repository.Migrations
{
    public partial class includeAccountNameforONB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "OnboardingRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "OnboardingRequests");
        }
    }
}
