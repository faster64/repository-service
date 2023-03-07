using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;

namespace KiotVietTimeSheet.Application.Validators.RuleValueValidators
{
    public class MainSalaryRuleValidator : MainSalaryRuleValueValidator
    {

        public MainSalaryRuleValidator(List<int> workBranchIds = null, List<Shift> shiftFromMainSalaries = null)
        {
            ValidateShift(workBranchIds, shiftFromMainSalaries ?? new List<Shift>());
        }

        protected void ValidateShift(List<int> workBranchIds, List<Shift> shiftFromMainSalaries)
        {
            RuleForEach(e => e.MainSalaryValueDetails)
                .Custom((detail, context) =>
                {
                    if (detail.ShiftId == 0) return;

                    var existShift = shiftFromMainSalaries.FirstOrDefault(x => x.Id == detail.ShiftId);

                    if (existShift == null)
                    {
                        context.AddFailure($"Ca làm việc không tồn tại");
                    }

                    if (existShift != null && existShift.IsDeleted)
                    {
                        context.AddFailure($"{existShift?.Name} đã xóa, vui lòng kiểm tra lại");
                    }

                    if (workBranchIds != null && !workBranchIds.Any(x => existShift != null && x == existShift.BranchId))
                    {
                        context.AddFailure($"Ca làm việc trong Thiết lập lương không thuộc chi nhánh làm việc của nhân viên");
                    }
                });
        }
    }
}
