using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using Microsoft.EntityFrameworkCore;
using ServiceStack;

namespace KiotVietTimeSheet.UnitTest
{
    public class InMemoryDb : IDisposable
    {
        public readonly EfDbContext Context; 
        public InMemoryDb()
        {
            Context = InMemoryContext();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }

        private static EfDbContext InMemoryContext()
        {
            var options = new DbContextOptionsBuilder<EfDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
//                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;
            var context = new EfDbContext(options);

            AddSampleData(context);

            return context;
        }


        private static void AddSampleData(EfDbContext context)
        {
            AddDepartment(context);
            AddEmployees(context);
            AddShifts(context);
            AddBranchSetting(context);
            AddPaysheet(context);
            AddTimeSheets(context);
            AddPayslips(context);
            AddClockings(context);
            AddPayslipClockings(context);
            AddAllowances(context);
            AddDeductions(context);
            AddPayRates(context);
            AddPayRateDetails(context);
            context.SaveChanges();
        }
        private static void AddEmployees(EfDbContext context, List<Employee> employees = null)
        {
            if (employees == null)
            {
                employees = new List<Employee>();
                for (int i = 1; i <= 5; i++)
                {
                    employees.Add(new Employee());
                }
                
            }

            if (employees.Any())
            {
                foreach (var employee in employees)
                {
                    employee.BranchId = 2;
                }

                context.Employees.AddRange(employees);
                context.SaveChanges();
            }
            
        }
        private static void AddDepartment(EfDbContext context, List<Department> departments = null)
        {
            if (departments == null)
            {
                departments = new List<Department>();
                for (int i = 1; i <= 10; i++)
                {
                    departments.Add(new Department(nameof(Department) + i, nameof(Department.Description) + i, true));
                }

            }

            if (departments.Any())
            {
                context.Departments.AddRange(departments);
            }

        }
        private static void AddShifts(EfDbContext context, List<Shift> shifts = null)
        {
            Random rnd = new Random();
            if (shifts == null)
            {
                shifts = new List<Shift>();
                for (int i = 1; i <= 5; i++)
                {
                    shifts.Add(new  Shift(nameof(Shift) + i, 10 * i, 20 * i, true, 2));
                }
            }

            if (shifts.Any())
            {
                context.Shifts.AddRange(shifts);
                context.SaveChanges();
            }
        }

        private static void AddBranchSetting(EfDbContext context, BranchSetting branchSetting = null)
        {
            if (branchSetting == null)
            {
                List<byte> workingDays = new List<byte>() { 1, 2, 3, 4 };
                branchSetting = new BranchSetting(2, workingDays);

                context.BranchSettings.AddRange(branchSetting);
            }
        }

        private static void AddTimeSheets(EfDbContext context, List<TimeSheet> timeSheets = null)
        {
            var employee = context.Employees.FirstOrDefault();
            var shiftIds = context.Shifts.Select(x => x.Id).ToList();
            var stringShiftIds = shiftIds.Join(",");
            if (timeSheets == null)
            {
                timeSheets = new List<TimeSheet>();
                for (int i = 0; i < 5; i++)
                {
                    if (employee != null)
                        timeSheets.Add(new TimeSheet(employee.Id, DateTime.Now, false, 1, 1, DateTime.Now, 2, false,
                            false,
                            new List<TimeSheetShift>()
                            {
                                new TimeSheetShift(1, stringShiftIds, "1,2")
                            }, "Noted"));
                }
            }

            if (timeSheets.Any())
            {
                context.TimeSheets.AddRange(timeSheets);
                context.SaveChanges();
            }
        }

        private static void AddClockings(EfDbContext context, List<Clocking> clockings = null)
        {
            var timeSheet = context.TimeSheets.FirstOrDefault();
            if (clockings == null)
            {
                clockings = new List<Clocking>();
                for (int i = 1; i <= 5; i++)
                {
                    if (timeSheet != null)
                        clockings.Add(new Clocking(i, timeSheet.Id, 1, 0 + i, 1, 1, DateTime.Now.AddDays(i),
                            DateTime.Now.AddDays(i),
                            "", 1, 2, 1, DateTime.Now,
                            null, null, false, null, null, null, null, 0, 0, 0, 0, null, new List<ClockingHistory>(), 0,
                            new Employee(), new Employee(), new TimeSheet()));
                }
            }

            if (clockings.Any())
            {
                context.Clockings.AddRange(clockings);
            }
        }

        private static void AddPaysheet(EfDbContext context, Paysheet paysheet = null)
        {
            if (paysheet == null)
            {
                paysheet = new Paysheet(1, "BL0001", 1, 2, false, null, null, 1, DateTime.Now, "BL", 1, DateTime.Now.AddDays(-1),
                    DateTime.Now.AddDays(20), 1, "", new List<Payslip>(), 20, null, null, "01/09/2019 - 30/09/2019", 1);

                context.Paysheet.AddRange(paysheet);
            }
        }

        private static void AddPayslips(EfDbContext context, List<Payslip> payslips = null)
        {
            if (payslips == null)
            {
                payslips = new List<Payslip>();
                for (int i = 1; i <= 5; i++)
                {
                    payslips.Add(new Payslip(0 + i, "PL00" + i, 1, 1, false, null, null, "", 1, 1, DateTime.Now, 1,
                        null, null, 0, 0, 0, 0, 0, 0, 0));
                }
            }

            if (payslips.Any())
            {
                context.Payslips.AddRange(payslips);
            }
        }

        private static void AddPayslipClockings(EfDbContext context, List<PayslipClocking> payslipClockings = null)
        {
            if (payslipClockings == null)
            {
                payslipClockings = new List<PayslipClocking>();
                for (int i = 1; i <= 5; i++)
                {
                    payslipClockings.Add(new PayslipClocking(0 + i, 1, null, null, 0, 0, 0, 0, null, 1, DateTime.Now, DateTime.Now, 1));
                }
            }

            if (payslipClockings.Any())
            {
                context.PayslipClockings.AddRange(payslipClockings);
            }
        }

        private static void AddAllowances(EfDbContext context, List<Allowance> allowances = null)
        {
            if (allowances == null)
            {
                allowances = new List<Allowance>();
                for (int i = 1; i <= 5; i++)
                {
                    allowances.Add(new Allowance("PC" + i));
                }
            }

            if (allowances.Any())
            {
                foreach (var allowance in allowances)
                {
                    allowance.TenantId = 1;
                }
                context.Allowance.AddRange(allowances);
            }
        }

        private static void AddDeductions(EfDbContext context, List<Deduction> deductions = null)
        {
            if (deductions == null)
            {
                deductions = new List<Deduction>();
                for (int i = 1; i <= 5; i++)
                {
                    deductions.Add(new Deduction("GT" + i));
                }
            }

            if (deductions.Any())
            {
                foreach (var deduction in deductions)
                {
                    deduction.TenantId = 1;
                }
                context.Deduction.AddRange(deductions);
            }
        }

        private static void AddPayRates(EfDbContext context, List<PayRate> payRates = null)
        {
            if (payRates == null)
            {
                payRates = new List<PayRate>();
                for (int i = 1; i <= 5; i++)
                {
                    payRates.Add(new PayRate(0 + i, null, 1, new List<IRule>()));
                }
            }

            if (payRates.Any())
            {
                context.PayRate.AddRange(payRates);
            }
        }

        private static void AddPayRateDetails(EfDbContext context, List<PayRateDetail> payRateDetails = null)
        {
            if (payRateDetails == null)
            {
                payRateDetails = new List<PayRateDetail>();
                for (int i = 1; i <= 5; i++)
                {
                    payRateDetails.Add(new PayRateDetail(0 + i, "MainSalaryRule",
                        "{\"Type\":2,\"MainSalaryValueDetails\":[{\"ShiftId\":0,\"Default\":10000.0,\"DayOff\":100.0,\"Holiday\":100.0,\"Rank\":0}]}",
                        1));
                }
            }

            if (payRateDetails.Any())
            {
                context.PayRateDetail.AddRange(payRateDetails);
            }
        }
    }
}
