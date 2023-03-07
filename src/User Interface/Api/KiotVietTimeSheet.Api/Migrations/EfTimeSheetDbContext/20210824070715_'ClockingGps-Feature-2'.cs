using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class ClockingGpsFeature2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RadiusLimit",
                table: "GpsInfo",
                type: "INT",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IdentityKeyClocking",
                table: "ConfirmClocking",
                type: "NVARCHAR(100)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RadiusLimit",
                table: "GpsInfo");

            migrationBuilder.DropColumn(
                name: "IdentityKeyClocking",
                table: "ConfirmClocking");
        }
    }
}
