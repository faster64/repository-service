using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class addNewClockingPenalizeAndPenalize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "ClockingPenalizeSeq",
                incrementBy: 1000);

            migrationBuilder.CreateSequence(
                name: "PenalizeSeq",
                incrementBy: 1000);

            migrationBuilder.CreateTable(
                name: "Penalize",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    Note = table.Column<string>(type: "NVARCHAR(512)", nullable: true),
                    Value = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penalize", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClockingPenalize",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ClockingId = table.Column<long>(type: "BIGINT", nullable: false),
                    PenalizeId = table.Column<long>(type: "BIGINT", nullable: false),
                    Value = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false),
                    TimesCount = table.Column<int>(type: "INT", nullable: false),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClockingPenalize", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClockingPenalize_Clocking_ClockingId",
                        column: x => x.ClockingId,
                        principalTable: "Clocking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClockingPenalize_Penalize_PenalizeId",
                        column: x => x.PenalizeId,
                        principalTable: "Penalize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-ClockingPenalize-ClockingId",
                table: "ClockingPenalize",
                column: "ClockingId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-ClockingPenalize-PenalizeId",
                table: "ClockingPenalize",
                column: "PenalizeId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-ClockingPenalize-TenantId",
                table: "ClockingPenalize",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Penalize-TenantId",
                table: "Penalize",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Penalize-TenantId-Code-Uniq",
                table: "Penalize",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClockingPenalize");

            migrationBuilder.DropTable(
                name: "CommissionDetail");

            migrationBuilder.DropTable(
                name: "Penalize");

            migrationBuilder.DropSequence(
                name: "ClockingPenalizeSeq");

            migrationBuilder.DropSequence(
                name: "PenalizeSeq");
        }
    }
}
