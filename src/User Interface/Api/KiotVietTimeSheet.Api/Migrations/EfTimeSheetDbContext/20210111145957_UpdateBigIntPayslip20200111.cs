using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class UpdateBigIntPayslip20200111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayslipClockingPenalize_Penalize_PenalizeId",
                table: "PayslipClockingPenalize");

            migrationBuilder.AlterColumn<long>(
                name: "PayslipId",
                table: "PayslipClockingPenalize",
                type: "BIGINT",
                nullable: false,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PayslipId",
                table: "PayslipClockingPenalize",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT");

            migrationBuilder.AddForeignKey(
                name: "FK_PayslipClockingPenalize_Penalize_PenalizeId",
                table: "PayslipClockingPenalize",
                column: "PenalizeId",
                principalTable: "Penalize",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
