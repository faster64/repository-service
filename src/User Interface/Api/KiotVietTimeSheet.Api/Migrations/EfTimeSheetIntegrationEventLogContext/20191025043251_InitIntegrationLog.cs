using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetIntegrationEventLogContext
{
    public partial class InitIntegrationLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntegrationEventLogEntry",
                columns: table => new
                {
                    EventId = table.Column<Guid>(nullable: false),
                    TransactionId = table.Column<Guid>(nullable: false),
                    EventType = table.Column<string>(nullable: false),
                    Data = table.Column<string>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()"),
                    State = table.Column<int>(nullable: false),
                    ProcessedTime = table.Column<DateTime>(nullable: true),
                    TimeSent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLogEntry", x => x.EventId);
                });

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-IntegrationEventLogEntry-State",
                table: "IntegrationEventLogEntry",
                column: "State");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationEventLogEntry");
        }
    }
}
