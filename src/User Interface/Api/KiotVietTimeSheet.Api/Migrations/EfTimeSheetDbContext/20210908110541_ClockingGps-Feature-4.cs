using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class ClockingGpsFeature4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Extra",
                table: "ConfirmClocking",
                type: "NVARCHAR(4000)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extra",
                table: "ConfirmClocking");
        }
    }
}
