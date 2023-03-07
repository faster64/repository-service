using System;
using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Validators;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.PayslipValidator
{
    public class CreateOrUpdatePayslipValidator : BasePayslipValidator<List<Payslip>>
    {
        #region Properties
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        #endregion

        public CreateOrUpdatePayslipValidator(
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
            )
            : base()
        {
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            ValidateExistCode();
            ValidateExistEmployee();
        }

        protected void ValidateExistCode()
        {
            RuleFor(p => p)
                .CustomAsync(async (payslips, context, token) =>
                {
                    var payslipsDuplicateCode = payslips.Where(p => !string.IsNullOrEmpty(p.Code))
                        .GroupBy(p => p.Code.ToLower()).Where(g => g.Count() > 1)
                        .ToDictionary(x => x.Key, y => y.ToList());
                    if (payslipsDuplicateCode.Any())
                    {
                        var employeeIds = payslipsDuplicateCode.Values.SelectMany(p => p).Select(p => p.EmployeeId)
                            .ToList();
                        var employees = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByIdsSpec(employeeIds), false, true);
                        context.AddFailure(string.Format(Message.payslip_duplicateCode, string.Join(", ", employees.Select(e => e.Name))));
                        return;
                    }

                    var payslipHaveCodes = payslips.Where(p => !string.IsNullOrEmpty(p.Code)).Distinct().ToList();
                    var existingPayslips = await _payslipReadOnlyRepository.GetBySpecificationAsync(new FindPayslipByCodesSpec(payslipHaveCodes.Select(x => x.Code).ToList()));
                    var firstDuplicateCodePayslip = existingPayslips.FirstOrDefault(e =>
                        payslips.Any(p => p.Code != null && p.Code.Equals(e.Code, StringComparison.OrdinalIgnoreCase) && p.Id != e.Id));

                    if (firstDuplicateCodePayslip != null)
                    {
                        var payslipHaveCode = payslipHaveCodes.FirstOrDefault(x => x.Code.Equals(firstDuplicateCodePayslip.Code, StringComparison.OrdinalIgnoreCase));
                        if (payslipHaveCode != null)
                        {
                            var employee = await _employeeReadOnlyRepository.FindBySpecificationAsync(new FindEmployeeByIdSpec(payslipHaveCode.EmployeeId));
                            context.AddFailure(string.Format(Message.payslip_existsCode, firstDuplicateCodePayslip.Code, employee.Name));
                        }
                    }
                });
        }

        protected void ValidateExistEmployee()
        {
            RuleFor(p => p)
                .Custom((payslips, context) =>
                {
                    if (payslips != null && payslips.Any())
                    {
                        // Kiểm xem xem có phiếu lương nào trùng nhân viên trong cùng một bảng lương
                        var employeeIds = payslips.Select(x => x.EmployeeId).ToList();
                        var duplicateEmployeeIds = employeeIds.GroupBy(x => x)
                            .Where(g => g.Count() > 1)
                            .Select(y => y.Key)
                            .ToList();
                        if (duplicateEmployeeIds.Any())
                        {
                            context.AddFailure(Message.paysheet_duplicateEmployee);
                        }
                    }

                });
        }
    }
}
