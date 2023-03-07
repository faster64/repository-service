using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KiotVietTimeSheet.Api.Migrations.EfTimeSheetDbContext
{
    public partial class InitDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "AllowanceSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "BranchSettingSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "ClockingHistorySeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "ClockingSeq",
                incrementBy: 8900);

            migrationBuilder.CreateSequence(
                name: "DeductionSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "DepartmentSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "EmployeeProfilePictureSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "EmployeeSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "FingerPrintSeq",
                incrementBy: 1000);

            migrationBuilder.CreateSequence(
                name: "HolidaySeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "JobTitleSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "PayRateSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "PayRateTemplateSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "PaysheetSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "PayslipSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "SettingsSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "ShiftSeq",
                incrementBy: 100);

            migrationBuilder.CreateSequence(
                name: "TimeSheetSeq",
                incrementBy: 1000);

            migrationBuilder.CreateTable(
                name: "Allowance",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    Code = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allowance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditProcessFailEvent",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<Guid>(nullable: false),
                    EventData = table.Column<string>(type: "NTEXT", nullable: false),
                    EventType = table.Column<string>(type: "NVARCHAR(500)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "NTEXT", nullable: false),
                    State = table.Column<int>(nullable: false),
                    RetryTimes = table.Column<int>(nullable: false),
                    ProcessedTime = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditProcessFailEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BranchSetting",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    BranchId = table.Column<int>(type: "INT", nullable: false),
                    WorkingDays = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    TenantId = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deduction",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    Code = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deduction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(500)", nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
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
                    table.PrimaryKey("PK_Department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FingerMachine",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    MachineName = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    MachineId = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    Vendor = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    Status = table.Column<int>(type: "INT", nullable: false, defaultValue: 1),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    Note = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    IpAddress = table.Column<string>(nullable: true),
                    Commpass = table.Column<int>(nullable: true),
                    Port = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FingerMachine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FingerPrint",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    FingerCode = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    EmployeeId = table.Column<long>(nullable: false),
                    FingerName = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    FingerNo = table.Column<int>(type: "INT", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FingerPrint", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holiday",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    From = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    To = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
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
                    table.PrimaryKey("PK_Holiday", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTitle",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(500)", nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
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
                    table.PrimaryKey("PK_JobTitle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NationalHoliday",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "NVARCHAR(250)", nullable: false),
                    StartDay = table.Column<byte>(type: "TINYINT", nullable: false),
                    EndDay = table.Column<byte>(type: "TINYINT", nullable: false),
                    StartMonth = table.Column<byte>(type: "TINYINT", nullable: false),
                    EndMonth = table.Column<byte>(type: "TINYINT", nullable: false),
                    IsLunarCalendar = table.Column<bool>(type: "BIT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NationalHoliday", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayRate",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    EmployeeId = table.Column<long>(type: "BIGINT", nullable: false),
                    PayRateTemplateId = table.Column<long>(type: "BIGINT", nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    SalaryPeriod = table.Column<byte>(type: "TINYINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayRate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayRateTemplate",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(250)", nullable: false),
                    SalaryPeriod = table.Column<byte>(type: "TINYINT", nullable: false),
                    BranchId = table.Column<int>(type: "INT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayRateTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paysheet",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    BranchId = table.Column<int>(type: "INT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    Name = table.Column<string>(nullable: true),
                    SalaryPeriod = table.Column<byte>(type: "TINYINT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    EndTime = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    PaysheetStatus = table.Column<byte>(type: "TINYINT", nullable: false),
                    Note = table.Column<string>(type: "NVARCHAR(2000)", nullable: true),
                    WorkingDayNumber = table.Column<int>(type: "INT", nullable: false),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    PaysheetPeriodName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    CreatorBy = table.Column<long>(type: "BIGINT", nullable: false),
                    PaysheetCreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    Version = table.Column<long>(type: "BIGINT", nullable: false, defaultValue: 0L),
                    IsDraft = table.Column<bool>(type: "BIT", nullable: false),
                    TimeOfStandardWorkingDay = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paysheet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    Value = table.Column<string>(type: "NVARCHAR(500)", nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shift",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(250)", nullable: false),
                    From = table.Column<long>(type: "BIGINT", nullable: false),
                    To = table.Column<long>(type: "BIGINT", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    BranchId = table.Column<int>(type: "INT", nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shift", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantNationalHoliday",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    LastCreatedYear = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantNationalHoliday", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    DOB = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    Gender = table.Column<bool>(type: "BIT", nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    IdentityNumber = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    MobilePhone = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    Facebook = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    LocationName = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    WardName = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    Note = table.Column<string>(type: "NVARCHAR(500)", nullable: true),
                    UserId = table.Column<long>(type: "BIGINT", nullable: true),
                    DepartmentId = table.Column<long>(type: "BIGINT", nullable: true),
                    JobTitleId = table.Column<long>(type: "BIGINT", nullable: true),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    BranchId = table.Column<int>(type: "INT", nullable: false),
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
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_JobTitle_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayRateDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PayRateId = table.Column<long>(nullable: false),
                    RuleType = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    RuleValue = table.Column<string>(type: "NTEXT", nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayRateDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayRateDetail_PayRate_PayRateId",
                        column: x => x.PayRateId,
                        principalTable: "PayRate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayRateTemplateDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    PayRateTemplateId = table.Column<long>(nullable: false),
                    RuleType = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    RuleValue = table.Column<string>(type: "NTEXT", nullable: false),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayRateTemplateDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayRateTemplateDetail_PayRateTemplate_PayRateTemplateId",
                        column: x => x.PayRateTemplateId,
                        principalTable: "PayRateTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payslip",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    PaysheetId = table.Column<long>(type: "BIGINT", nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    Note = table.Column<string>(type: "NVARCHAR(2000)", nullable: true),
                    PayslipStatus = table.Column<byte>(type: "TINYINT", nullable: false),
                    EmployeeId = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    MainSalary = table.Column<decimal>(type: "MONEY", nullable: false),
                    CommissionSalary = table.Column<decimal>(type: "MONEY", nullable: false),
                    OvertimeSalary = table.Column<decimal>(type: "MONEY", nullable: false),
                    Allowance = table.Column<decimal>(type: "MONEY", nullable: false),
                    Deduction = table.Column<decimal>(type: "MONEY", nullable: false),
                    Bonus = table.Column<decimal>(type: "MONEY", nullable: false),
                    NetSalary = table.Column<decimal>(type: "MONEY", nullable: false),
                    GrossSalary = table.Column<decimal>(type: "MONEY", nullable: false),
                    TotalPayment = table.Column<decimal>(type: "MONEY", nullable: false, defaultValueSql: "0"),
                    IsDraft = table.Column<bool>(type: "BIT", nullable: false),
                    PayslipCreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    PayslipCreatedBy = table.Column<long>(type: "BIGINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payslip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payslip_Paysheet_PaysheetId",
                        column: x => x.PaysheetId,
                        principalTable: "Paysheet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clocking",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    TimeSheetId = table.Column<long>(type: "BIGINT", nullable: false),
                    ShiftId = table.Column<long>(type: "BIGINT", nullable: false),
                    EmployeeId = table.Column<long>(type: "BIGINT", nullable: false),
                    WorkById = table.Column<long>(type: "BIGINT", nullable: false),
                    ClockingStatus = table.Column<byte>(type: "TINYINT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    EndTime = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    Note = table.Column<string>(type: "NVARCHAR(500)", nullable: true),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    BranchId = table.Column<int>(type: "INT", nullable: false),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    CheckInDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    CheckOutDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    TimeIsLate = table.Column<int>(type: "INT", nullable: false, defaultValue: 0),
                    OverTimeBeforeShiftWork = table.Column<int>(type: "INT", nullable: false, defaultValue: 0),
                    TimeIsLeaveWorkEarly = table.Column<int>(type: "INT", nullable: false, defaultValue: 0),
                    OverTimeAfterShiftWork = table.Column<int>(type: "INT", nullable: false, defaultValue: 0),
                    AbsenceType = table.Column<byte>(type: "TINYINT", nullable: true),
                    ClockingPaymentStatus = table.Column<byte>(type: "TINYINT", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clocking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clocking_Employee_WorkById",
                        column: x => x.WorkById,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfilePicture",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    EmployeeId = table.Column<long>(type: "BIGINT", nullable: false),
                    ImageUrl = table.Column<string>(type: "NVARCHAR(4000)", nullable: false),
                    IsMainImage = table.Column<bool>(type: "BIT", nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfilePicture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeProfilePicture_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeSheet",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    EmployeeId = table.Column<long>(type: "BIGINT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    IsRepeat = table.Column<bool>(type: "BIT", nullable: true),
                    RepeatType = table.Column<byte>(type: "TINYINT", nullable: true),
                    RepeatEachDay = table.Column<byte>(type: "TINYINT", nullable: true),
                    BranchId = table.Column<int>(type: "INT", nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    TimeSheetStatus = table.Column<byte>(type: "TINYINT", nullable: false, defaultValueSql: "1"),
                    SaveOnDaysOffOfBranch = table.Column<bool>(type: "BIT", nullable: false),
                    SaveOnHoliday = table.Column<bool>(type: "BIT", nullable: false),
                    Note = table.Column<string>(type: "NVARCHAR(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSheet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSheet_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayslipClocking",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PayslipId = table.Column<long>(nullable: false),
                    ClockingId = table.Column<long>(nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    CheckOutDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    TimeIsLate = table.Column<int>(type: "INT", nullable: false),
                    OverTimeBeforeShiftWork = table.Column<int>(type: "INT", nullable: false),
                    TimeIsLeaveWorkEarly = table.Column<int>(type: "INT", nullable: false),
                    OverTimeAfterShiftWork = table.Column<int>(type: "INT", nullable: false),
                    AbsenceType = table.Column<byte>(type: "TINYINT", nullable: true),
                    ClockingStatus = table.Column<byte>(type: "TINYINT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    EndTime = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false),
                    ShiftId = table.Column<long>(type: "BIGINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayslipClocking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayslipClocking_Payslip_PayslipId",
                        column: x => x.PayslipId,
                        principalTable: "Payslip",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayslipDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PayslipId = table.Column<long>(nullable: false),
                    RuleType = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    RuleValue = table.Column<string>(type: "NTEXT", nullable: false),
                    RuleParam = table.Column<string>(type: "NTEXT", nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayslipDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayslipDetail_Payslip_PayslipId",
                        column: x => x.PayslipId,
                        principalTable: "Payslip",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClockingHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ClockingId = table.Column<long>(type: "BIGINT", nullable: false),
                    CheckedInDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    CheckedOutDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    TimeIsLate = table.Column<int>(type: "INT", nullable: false),
                    OverTimeBeforeShiftWork = table.Column<int>(type: "INT", nullable: false),
                    TimeIsLeaveWorkEarly = table.Column<int>(type: "INT", nullable: false),
                    OverTimeAfterShiftWork = table.Column<int>(type: "INT", nullable: false),
                    TenantId = table.Column<int>(type: "INT", nullable: false),
                    BranchId = table.Column<int>(type: "INT", nullable: false),
                    TimeKeepingType = table.Column<byte>(type: "TINYINT", nullable: false, defaultValueSql: "1"),
                    ClockingStatus = table.Column<byte>(type: "TINYINT", nullable: false, defaultValueSql: "2"),
                    ModifiedBy = table.Column<long>(type: "BIGINT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    CreatedBy = table.Column<long>(type: "BIGINT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false, defaultValueSql: "GETDATE()"),
                    ClockingHistoryStatus = table.Column<byte>(type: "TINYINT", nullable: false, defaultValueSql: "1"),
                    TimeIsLateAdjustment = table.Column<int>(type: "INT", nullable: false),
                    OverTimeBeforeShiftWorkAdjustment = table.Column<int>(type: "INT", nullable: false),
                    TimeIsLeaveWorkEarlyAdjustment = table.Column<int>(type: "INT", nullable: false),
                    OverTimeAfterShiftWorkAdjustment = table.Column<int>(type: "INT", nullable: false),
                    AbsenceType = table.Column<byte>(type: "TINYINT", nullable: true),
                    ShiftId = table.Column<long>(type: "BIGINT", nullable: true),
                    ShiftFrom = table.Column<long>(type: "BIGINT", nullable: true),
                    ShiftTo = table.Column<long>(type: "BIGINT", nullable: true),
                    EmployeeId = table.Column<long>(type: "BIGINT", nullable: true),
                    CheckTime = table.Column<DateTime>(type: "DATETIME2(3)", nullable: true),
                    AutoTimekeepingUid = table.Column<string>(type: "NVARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClockingHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClockingHistory_Clocking_ClockingId",
                        column: x => x.ClockingId,
                        principalTable: "Clocking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeSheetShift",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeSheetId = table.Column<long>(type: "BIGINT", nullable: false),
                    ShiftIds = table.Column<string>(type: "NVARCHAR(1000)", nullable: false),
                    RepeatDaysOfWeek = table.Column<string>(type: "NVARCHAR(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSheetShift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSheetShift_TimeSheet_TimeSheetId",
                        column: x => x.TimeSheetId,
                        principalTable: "TimeSheet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Allowance-TenantId",
                table: "Allowance",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Allowance-TenantId-Code-Uniq",
                table: "Allowance",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-AuditProcessFailEvent-EventId",
                table: "AuditProcessFailEvent",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-AuditProcessFailEvent-State",
                table: "AuditProcessFailEvent",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-BranchSetting-BranchId",
                table: "BranchSetting",
                column: "BranchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-BranchSetting-TenantId",
                table: "BranchSetting",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Clocking-BranchId",
                table: "Clocking",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Clocking-EmployeeId",
                table: "Clocking",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Clocking-ShiftId",
                table: "Clocking",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Clocking-TenantId",
                table: "Clocking",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Clocking-TimeSheetId",
                table: "Clocking",
                column: "TimeSheetId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Clocking-WorkById",
                table: "Clocking",
                column: "WorkById");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-ClockingHistory-BranchId",
                table: "ClockingHistory",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-ClockingHistory-ClockingId",
                table: "ClockingHistory",
                column: "ClockingId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-ClockingHistory-TenantId",
                table: "ClockingHistory",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "UniqueIndex-ClockingHistory-AutoTimekeepingUid",
                table: "ClockingHistory",
                columns: new[] { "TenantId", "ClockingId", "AutoTimekeepingUid" },
                unique: true,
                filter: "[AutoTimekeepingUid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Deduction-TenantId",
                table: "Deduction",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Deduction-TenantId-Code-Uniq",
                table: "Deduction",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Department-TenantId",
                table: "Department",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Employee-BranchId",
                table: "Employee",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DepartmentId",
                table: "Employee",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_JobTitleId",
                table: "Employee",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Employee-TenantId-Code-Uniq",
                table: "Employee",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-EmployeeProfilePicture-EmployeeId",
                table: "EmployeeProfilePicture",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-EmployeeProfilePicture-TenantId",
                table: "EmployeeProfilePicture",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-FingerMachine-BranchId",
                table: "FingerMachine",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-FingerMachine-TenantId",
                table: "FingerMachine",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FingerMachine_TenantId_MachineId",
                table: "FingerMachine",
                columns: new[] { "TenantId", "MachineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-FingerPrint-BranchId",
                table: "FingerPrint",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-FingerPrint-EmployeeId",
                table: "FingerPrint",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-FingerPrint-TenantId",
                table: "FingerPrint",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Holiday-TenantId",
                table: "Holiday",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-JobTitle-TenantId",
                table: "JobTitle",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayRate-EmployeeId",
                table: "PayRate",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayRate-TenantId",
                table: "PayRate",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayRateDetail-PayRateId",
                table: "PayRateDetail",
                column: "PayRateId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayRateTemplate-BranchId",
                table: "PayRateTemplate",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayRateTemplate-TenantId",
                table: "PayRateTemplate",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayRateTemplateDetail-PayRateTemplateId",
                table: "PayRateTemplateDetail",
                column: "PayRateTemplateId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Paysheet-BranchId",
                table: "Paysheet",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Paysheet-TenantId",
                table: "Paysheet",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Paysheet-TenantId-Code-Uniq",
                table: "Paysheet",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Payslip-EmployeeId",
                table: "Payslip",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Payslip-PaysheetId",
                table: "Payslip",
                column: "PaysheetId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Payslip-TenantId",
                table: "Payslip",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Payslip-TenantId-Code-Uniq",
                table: "Payslip",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayslipClocking-ClockingId",
                table: "PayslipClocking",
                column: "ClockingId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayslipClocking-PayslipId",
                table: "PayslipClocking",
                column: "PayslipId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-PayslipDetail-PayslipId",
                table: "PayslipDetail",
                column: "PayslipId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-BranchSetting-TenantId",
                table: "Settings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Shift-BranchId",
                table: "Shift",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-Shift-TenantId",
                table: "Shift",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-TenantNationalHoliday-TenantId",
                table: "TenantNationalHoliday",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-TimeSheet-BranchId",
                table: "TimeSheet",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-TimeSheet-EmployeeId",
                table: "TimeSheet",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-TimeSheet-TenantId",
                table: "TimeSheet",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-TimeSheet-TimeSheetShift",
                table: "TimeSheetShift",
                column: "TimeSheetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Allowance");

            migrationBuilder.DropTable(
                name: "AuditProcessFailEvent");

            migrationBuilder.DropTable(
                name: "BranchSetting");

            migrationBuilder.DropTable(
                name: "ClockingHistory");

            migrationBuilder.DropTable(
                name: "Deduction");

            migrationBuilder.DropTable(
                name: "EmployeeProfilePicture");

            migrationBuilder.DropTable(
                name: "FingerMachine");

            migrationBuilder.DropTable(
                name: "FingerPrint");

            migrationBuilder.DropTable(
                name: "Holiday");

            migrationBuilder.DropTable(
                name: "NationalHoliday");

            migrationBuilder.DropTable(
                name: "PayRateDetail");

            migrationBuilder.DropTable(
                name: "PayRateTemplateDetail");

            migrationBuilder.DropTable(
                name: "PayslipClocking");

            migrationBuilder.DropTable(
                name: "PayslipDetail");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Shift");

            migrationBuilder.DropTable(
                name: "TenantNationalHoliday");

            migrationBuilder.DropTable(
                name: "TimeSheetShift");

            migrationBuilder.DropTable(
                name: "Clocking");

            migrationBuilder.DropTable(
                name: "PayRate");

            migrationBuilder.DropTable(
                name: "PayRateTemplate");

            migrationBuilder.DropTable(
                name: "Payslip");

            migrationBuilder.DropTable(
                name: "TimeSheet");

            migrationBuilder.DropTable(
                name: "Paysheet");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "JobTitle");

            migrationBuilder.DropSequence(
                name: "AllowanceSeq");

            migrationBuilder.DropSequence(
                name: "BranchSettingSeq");

            migrationBuilder.DropSequence(
                name: "ClockingHistorySeq");

            migrationBuilder.DropSequence(
                name: "ClockingSeq");

            migrationBuilder.DropSequence(
                name: "DeductionSeq");

            migrationBuilder.DropSequence(
                name: "DepartmentSeq");

            migrationBuilder.DropSequence(
                name: "EmployeeProfilePictureSeq");

            migrationBuilder.DropSequence(
                name: "EmployeeSeq");

            migrationBuilder.DropSequence(
                name: "FingerPrintSeq");

            migrationBuilder.DropSequence(
                name: "HolidaySeq");

            migrationBuilder.DropSequence(
                name: "JobTitleSeq");

            migrationBuilder.DropSequence(
                name: "PayRateSeq");

            migrationBuilder.DropSequence(
                name: "PayRateTemplateSeq");

            migrationBuilder.DropSequence(
                name: "PaysheetSeq");

            migrationBuilder.DropSequence(
                name: "PayslipSeq");

            migrationBuilder.DropSequence(
                name: "SettingsSeq");

            migrationBuilder.DropSequence(
                name: "ShiftSeq");

            migrationBuilder.DropSequence(
                name: "TimeSheetSeq");
        }
    }
}
