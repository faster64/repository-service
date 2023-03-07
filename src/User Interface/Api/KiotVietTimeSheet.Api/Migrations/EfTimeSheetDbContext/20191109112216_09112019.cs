using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class _09112019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Vendor",
                table: "FingerMachine",
                type: "NVARCHAR(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(255)");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "FingerMachine",
                type: "VARCHAR(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(100)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConnectionType",
                table: "FingerMachine",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionType",
                table: "FingerMachine");

            migrationBuilder.AlterColumn<string>(
                name: "Vendor",
                table: "FingerMachine",
                type: "NVARCHAR(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(255)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "FingerMachine",
                type: "NVARCHAR(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)",
                oldNullable: true);
        }
    }
}
