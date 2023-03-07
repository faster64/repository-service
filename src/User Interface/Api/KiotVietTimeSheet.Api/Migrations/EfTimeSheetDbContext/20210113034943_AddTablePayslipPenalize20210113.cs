using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class AddTablePayslipPenalize20210113 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "PayslipPenalizeSeq",
                incrementBy: 1000);

            migrationBuilder.CreateTable(
                name: "PayslipPenalize",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    PayslipId = table.Column<long>(type: "BIGINT", nullable: false),
                    PenalizeId = table.Column<long>(type: "BIGINT", nullable: false),
                    PenalizeName = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    Value = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false),
                    TimesCount = table.Column<int>(type: "INT", nullable: false),
                    MoneyType = table.Column<int>(type: "INT", nullable: false, defaultValue: 1),
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
                    table.PrimaryKey("PK_PayslipPenalize", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayslipPenalize_Payslip_PayslipId",
                        column: x => x.PayslipId,
                        principalTable: "Payslip",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayslipPenalize-PayslipId",
                table: "PayslipPenalize",
                column: "PayslipId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayslipPenalize");

            migrationBuilder.DropSequence(
                name: "PayslipPenalizeSeq");
        }
    }
}
