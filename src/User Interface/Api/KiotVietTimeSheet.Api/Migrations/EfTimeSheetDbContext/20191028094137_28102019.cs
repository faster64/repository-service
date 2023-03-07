using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class _28102019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Port",
                table: "FingerMachine",
                type: "INT",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "FingerMachine",
                type: "NVARCHAR(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Commpass",
                table: "FingerMachine",
                type: "INT",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Port",
                table: "FingerMachine",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "FingerMachine",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(500)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Commpass",
                table: "FingerMachine",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INT",
                oldNullable: true);
        }
    }
}
