using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class _20200105_delete_column_note : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Penalize");

            migrationBuilder.AddColumn<int>(
                name: "MoneyType",
                table: "ClockingPenalize",
                type: "INT",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoneyType",
                table: "ClockingPenalize");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Penalize",
                type: "NVARCHAR(512)",
                nullable: true);
        }
    }
}
