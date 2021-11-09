using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowStone.Repository.Migrations
{
    public partial class AddedCustomerManagementColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActionReason",
                table: "CustomerRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CallingPhoneNumber",
                table: "CustomerRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionReason",
                table: "CustomerRequests");

            migrationBuilder.DropColumn(
                name: "CallingPhoneNumber",
                table: "CustomerRequests");
        }
    }
}
