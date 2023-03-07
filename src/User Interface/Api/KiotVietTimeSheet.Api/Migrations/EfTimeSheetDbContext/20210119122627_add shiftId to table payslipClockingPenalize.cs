using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class addshiftIdtotablepayslipClockingPenalize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "PayslipPenalize",
                type: "BIT",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "BIT",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<long>(
                name: "ShiftId",
                table: "PayslipClockingPenalize",
                type: "BIGINT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "PayslipClockingPenalize");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "PayslipPenalize",
                type: "BIT",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "BIT");
        }
    }
}
