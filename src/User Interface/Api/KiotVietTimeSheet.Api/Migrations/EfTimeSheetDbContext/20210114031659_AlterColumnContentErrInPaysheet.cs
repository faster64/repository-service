using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class AlterColumnContentErrInPaysheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentErr",
                table: "Payslip");

            migrationBuilder.AddColumn<string>(
                name: "ContentErr",
                table: "Paysheet",
                type: "NVARCHAR(512)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentErr",
                table: "Paysheet");

            migrationBuilder.AddColumn<string>(
                name: "ContentErr",
                table: "Payslip",
                type: "NVARCHAR(512)",
                nullable: true);
        }
    }
}
