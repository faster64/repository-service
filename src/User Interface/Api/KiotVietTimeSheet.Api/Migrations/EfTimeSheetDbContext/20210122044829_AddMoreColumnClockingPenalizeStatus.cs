using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class AddMoreColumnClockingPenalizeStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ClockingPaymentStatus",
                table: "ClockingPenalize",
                type: "TINYINT",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClockingPaymentStatus",
                table: "ClockingPenalize");
        }
    }
}
