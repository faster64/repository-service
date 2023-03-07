using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class UpdateTypeForPaysheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentErr",
                table: "Paysheet");

            migrationBuilder.AddColumn<int>(
                name: "ErrorStatus",
                table: "Paysheet",
                type: "INT",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorStatus",
                table: "Paysheet");

            migrationBuilder.AddColumn<string>(
                name: "ContentErr",
                table: "Paysheet",
                type: "NVARCHAR(512)",
                nullable: true);
        }
    }
}
