using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class PayslipClockingPenalize_20200108 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "PayslipClockingPenalizeSeq",
                incrementBy: 1000);

            migrationBuilder.CreateTable(
                name: "PayslipClockingPenalize",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    PayslipId = table.Column<long>(nullable: false),
                    ClockingId = table.Column<long>(type: "BIGINT", nullable: false),
                    PenalizeId = table.Column<long>(type: "BIGINT", nullable: false),
                    PenalizeName = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    Value = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false),
                    TimesCount = table.Column<int>(type: "INT", nullable: false),
                    MoneyType = table.Column<int>(type: "INT", nullable: false, defaultValue: 1),
                    ClockingPenalizeCreated = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayslipClockingPenalize", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayslipClockingPenalize_Payslip_PayslipId",
                        column: x => x.PayslipId,
                        principalTable: "Payslip",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayslipClockingPenalize_Penalize_PenalizeId",
                        column: x => x.PenalizeId,
                        principalTable: "Penalize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayslipClockingPenalize-PayslipId",
                table: "PayslipClockingPenalize",
                column: "PayslipId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayslipClockingPenalize-PenalizeId",
                table: "PayslipClockingPenalize",
                column: "PenalizeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayslipClockingPenalize");

            migrationBuilder.DropSequence(
                name: "PayslipClockingPenalizeSeq");
        }
    }
}
