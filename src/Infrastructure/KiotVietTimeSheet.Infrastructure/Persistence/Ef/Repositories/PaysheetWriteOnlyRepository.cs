using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using Microsoft.EntityFrameworkCore;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class PaysheetWriteOnlyRepository : EfBaseWriteOnlyRepository<Paysheet>, IPaysheetWriteOnlyRepository
    {
        #region Properties 
        private readonly IPayslipWriteOnlyRepository _payslipWriteOnlyRepository;
        private readonly IPayslipDetailWriteOnlyRepository _payslipDetailWriteOnlyRepository;
        private readonly IPayslipClockingWriteOnlyRepository _payslipClockingWriteOnlyRepository;
        private readonly IPayslipClockingPenalizeWriteOnlyRepository _payslipClockingPenalizeWriteOnlyRepository;
        private readonly IPayslipPenalizeWriteOnlyRepository _payslipPenalizeWriteOnlyRepository;
        #endregion
        public PaysheetWriteOnlyRepository(
            EfDbContext db,
            IAuthService authService,
            ILogger<PaysheetWriteOnlyRepository> logger,
            IPayslipWriteOnlyRepository payslipWriteOnlyRepository,
            IPayslipDetailWriteOnlyRepository payslipDetailWriteOnlyRepository,
            IPayslipClockingWriteOnlyRepository payslipClockingWriteOnlyRepository,
            IPayslipClockingPenalizeWriteOnlyRepository payslipClockingPenalizeWriteOnlyRepository,
            IPayslipPenalizeWriteOnlyRepository payslipPenalizeWriteOnlyRepository) : base(db, authService, logger)
        {
            _payslipWriteOnlyRepository = payslipWriteOnlyRepository;
            _payslipDetailWriteOnlyRepository = payslipDetailWriteOnlyRepository;
            _payslipClockingWriteOnlyRepository = payslipClockingWriteOnlyRepository;
            _payslipClockingPenalizeWriteOnlyRepository = payslipClockingPenalizeWriteOnlyRepository;
            _payslipPenalizeWriteOnlyRepository = payslipPenalizeWriteOnlyRepository;
        }

        public async Task<Paysheet> UpdatePaysheetAsync(Paysheet paysheet)
        {
            var existPaysheet = await Db.Paysheet.Include(p => p.Payslips).FirstOrDefaultAsync(p => p.Id == paysheet.Id);
            if (existPaysheet == null)
            {
                var ex = new Exception($"Không tìm thấy bảng lương {paysheet.Id}");
                throw ex;
            }

            Db.Entry(existPaysheet).CurrentValues.SetValues(paysheet);
            return paysheet;
        }

        public async Task<Paysheet> StoreAsync(Paysheet paysheet, bool isUpdate = false)
        {
            if (paysheet.Id <= 0)
            {
                Add(paysheet);
                return paysheet;
            }

            var payslipIds = paysheet.Payslips.Select(ps => ps.Id).ToList();
            var existingPayslips =
                await Db.Payslips.Where(p => payslipIds.Contains(p.Id))
                    .Include(p => p.PayslipDetails)
                    .Include(x => x.PayslipPenalizes)
                    .ToListAsync();

            var payslipsNeedAdd = new List<Payslip>();
            var payslipsNeedUpdate = new List<Payslip>();
            var payslipDetailsNeedDelete = new List<PayslipDetail>();
            var payslipDetailsNeedAdd = new List<PayslipDetail>();
            var payslipPenalizeNeedAdd = new List<PayslipPenalize>();
            var payslipPenalizeNeedDelete = new List<PayslipPenalize>();
            _payslipWriteOnlyRepository.GenerateRangeNextCode(paysheet.Payslips);
            // insert or update payslip
            foreach (var payslip in paysheet.Payslips)
            {
                var existingPayslip = existingPayslips.SingleOrDefault(x => x.Id == payslip.Id);
                if (existingPayslip == null)
                {
                    payslipsNeedAdd.Add(payslip);
                    continue;
                }

                if (payslip.PayslipDetails != null && payslip.PayslipDetails.Any())
                {
                    payslipDetailsNeedDelete.AddRange(existingPayslip.PayslipDetails);
                    payslipDetailsNeedAdd.AddRange(payslip.PayslipDetails);
                }
                payslipsNeedUpdate.Add(payslip);

                var payslipNeedAccess =
                    AddPayslipPenalizeNeedUse(payslip.PayslipPenalizes, existingPayslip.PayslipPenalizes);

                payslipPenalizeNeedAdd.AddRange(payslipNeedAccess.Item1);
                payslipPenalizeNeedDelete.AddRange(payslipNeedAccess.Item2);
            }

            _payslipDetailWriteOnlyRepository.BatchDelete(payslipDetailsNeedDelete);
            _payslipDetailWriteOnlyRepository.BatchAdd(payslipDetailsNeedAdd);
            _payslipWriteOnlyRepository.BatchAdd(payslipsNeedAdd, new[] { nameof(Payslip.TotalPayment) });
            _payslipWriteOnlyRepository.BatchUpdate(payslipsNeedUpdate, new[] { nameof(Payslip.TotalPayment) });

            var payslipNeedProcess = payslipsNeedAdd.Concat(payslipsNeedUpdate).ToList();

            if (!isUpdate)
            {
                var payslipIdExists = existingPayslips.Select(x => x.Id).ToList();
                await _payslipPenalizeWriteOnlyRepository.DeleteAllAsync(payslipIdExists);
            }
            else
            {
                _payslipPenalizeWriteOnlyRepository.BatchDelete(payslipPenalizeNeedDelete);
            }

            await _payslipClockingPenalizeWriteOnlyRepository.CreateOrUpdateAsync(payslipNeedProcess);
            await _payslipClockingWriteOnlyRepository.CreateOrUpdateAsync(payslipNeedProcess, paysheet.StartTime, paysheet.EndTime);
            _payslipPenalizeWriteOnlyRepository.BatchAdd(payslipPenalizeNeedAdd);
            Update(paysheet);


            return paysheet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payslipPenalizes">new payslip penalize</param>
        /// <param name="existListPayslipPenalize">old payslip penalize</param>
        /// <returns></returns>
        private Tuple<List<PayslipPenalize>, List<PayslipPenalize>> AddPayslipPenalizeNeedUse(
            List<PayslipPenalize> payslipPenalizes, List<PayslipPenalize> existListPayslipPenalize)
        {
            var payslipPenalizeNeedAdd = new List<PayslipPenalize>();
            var payslipPenalizeNeedDelete = new List<PayslipPenalize>();

            if (payslipPenalizes == null || !payslipPenalizes.Any()) return new Tuple<List<PayslipPenalize>, List<PayslipPenalize>>(new List<PayslipPenalize>(), new List<PayslipPenalize>());

            payslipPenalizeNeedDelete.AddRange(existListPayslipPenalize);
            payslipPenalizeNeedAdd.AddRange(payslipPenalizes);

            return new Tuple<List<PayslipPenalize>, List<PayslipPenalize>>(payslipPenalizeNeedAdd, payslipPenalizeNeedDelete);
        }

        public async Task<List<Paysheet>> GetPaysheetDraftAndTempByEmployeeIds(List<long> employeeIds)
        {
            var existPaysheetIds = Db.Payslips
                .Where(x => x.PayslipStatus != (byte)PayslipStatuses.PaidSalary &&
                            x.PayslipStatus != (byte)PayslipStatuses.Void)
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .Select(x => x.PaysheetId).ToList();

            return await Db.Paysheet
                .Where(x => existPaysheetIds.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<List<Paysheet>> GetPaysheetDraftAndTempByClockings(List<Clocking> clockings)
        {
            var existPaysheetIds = Db.Payslips
                .Where(x => x.PayslipStatus != (byte)PayslipStatuses.PaidSalary &&
                            x.PayslipStatus != (byte)PayslipStatuses.Void)
                .Where(x => clockings.Select(c => c.EmployeeId).Contains(x.EmployeeId))
                .Select(x => x.PaysheetId).ToList();

            return await Db.Paysheet
                .Where(x => existPaysheetIds.Contains(x.Id))
                .Where(x => clockings.Any(c => x.StartTime <= c.StartTime && x.EndTime > c.EndTime))
                .ToListAsync();
        }

        public async Task<List<Paysheet>> GetPaysheetDraftAndTempByCommissionIds(List<long> commissionIds)
        {
            var payslipIds = (await Db.PayslipDetails
                .Where(x => x.TenantId == AuthService.Context.TenantId && x.RuleType == nameof(CommissionSalaryRuleV2))
                .ToListAsync())
                .Where(x =>
                {
                    var rule = RuleFactory.GetRule(x.RuleType, x.RuleValue);
                    var isCommissionRule = rule?.GetType() == typeof(CommissionSalaryRuleV2);
                    if (rule == null || !isCommissionRule) return false;

                    var ruleParam = JsonConvert.DeserializeObject(x.RuleParam, typeof(CommissionSalaryRuleParamV2)) as CommissionSalaryRuleParamV2;
                    if (ruleParam == null || ruleParam.Type != CommissionSalaryTypes.WithTotalCommission) return false;

                    var isUsingCommissionTable = ruleParam.CommissionParams.Any(cp =>
                        cp.CommissionTable != null && commissionIds.Contains(cp.CommissionTable.Id));
                    return isUsingCommissionTable;
                })
                .Select(x => x.PayslipId)
                .Distinct();

            var paysheetIds = Db.Payslips
                .Where(x => x.TenantId == AuthService.Context.TenantId)
                .Where(x => x.PayslipStatus != (byte)PayslipStatuses.PaidSalary &&
                            x.PayslipStatus != (byte)PayslipStatuses.Void)
                .Where(x => payslipIds.Contains(x.Id))
                .Select(x => x.PaysheetId);

            var paysheets = await Db.Paysheet
                .Where(x => x.TenantId == AuthService.Context.TenantId)
                .Where(x => paysheetIds.Contains(x.Id))
                .ToListAsync();

            return paysheets;
        }
    }
}
