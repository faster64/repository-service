using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class ClockingGpsFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "ConfirmClockingHistorySeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "ConfirmClockingSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "GpsInfoSeq",
                incrementBy: 100);

            migrationBuilder.AddColumn<string>(
                name: "IdentityKeyClocking",
                table: "Employee",
                type: "NVARCHAR(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GpsInfo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    BranchId = table.Column<int>(type: "INT", nullable: false),
                    Coordinate = table.Column<string>(type: "NVARCHAR(2000)", nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    LocationName = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    WardName = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    Province = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    District = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    Status = table.Column<byte>(type: "TINYINT", nullable: false),
                    QrKey = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpsInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmClocking",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    GpsInfoId = table.Column<long>(type: "BIGINT", nullable: false),
                    EmployeeId = table.Column<long>(type: "BIGINT", nullable: false),
                    CheckTime = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    Type = table.Column<byte>(type: "TINYINT", nullable: false),
                    Status = table.Column<byte>(type: "TINYINT", nullable: false),
                    Note = table.Column<string>(type: "NVARCHAR(2000)", nullable: true),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmClocking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfirmClocking_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfirmClocking_GpsInfo_GpsInfoId",
                        column: x => x.GpsInfoId,
                        principalTable: "GpsInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmClockingHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    ConfirmClockingId = table.Column<long>(type: "BIGINT", nullable: false),
                    ConfirmBy = table.Column<long>(type: "BIGINT", nullable: false),
                    ConfirmDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    StatusOld = table.Column<byte>(type: "TINYINT", nullable: false),
                    StatusNew = table.Column<byte>(type: "TINYINT", nullable: false),
                    Note = table.Column<string>(type: "NVARCHAR(2000)", nullable: true),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmClockingHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfirmClockingHistory_ConfirmClocking_ConfirmClockingId",
                        column: x => x.ConfirmClockingId,
                        principalTable: "ConfirmClocking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmClocking_EmployeeId",
                table: "ConfirmClocking",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-ConfirmClocking-GpsInfoId",
                table: "ConfirmClocking",
                column: "GpsInfoId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-ConfirmClocking-TenantId",
                table: "ConfirmClocking",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-ConfirmClockingHistory-ConfirmClockingId",
                table: "ConfirmClockingHistory",
                column: "ConfirmClockingId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-ConfirmClockingHistory-TenantId",
                table: "ConfirmClockingHistory",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-GpsInfo-TenantId",
                table: "GpsInfo",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfirmClockingHistory");

            migrationBuilder.DropTable(
                name: "ConfirmClocking");

            migrationBuilder.DropTable(
                name: "GpsInfo");

            migrationBuilder.DropSequence(
                name: "ConfirmClockingHistorySeq");

            migrationBuilder.DropSequence(
                name: "ConfirmClockingSeq");

            migrationBuilder.DropSequence(
                name: "GpsInfoSeq");

            migrationBuilder.DropColumn(
                name: "IdentityKeyClocking",
                table: "Employee");
        }
    }
}
