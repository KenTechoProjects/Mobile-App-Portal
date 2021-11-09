using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowStone.Repository.Migrations
{
    public partial class BranchCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BranchName",
                table: "OnboardingRequests",
                newName: "RequestBranchCode");

            migrationBuilder.RenameColumn(
                name: "BranchCode",
                table: "OnboardingRequests",
                newName: "AccountBranchCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestBranchCode",
                table: "OnboardingRequests",
                newName: "BranchName");

            migrationBuilder.RenameColumn(
                name: "AccountBranchCode",
                table: "OnboardingRequests",
                newName: "BranchCode");
        }
    }
}
