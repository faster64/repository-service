using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Validators
{
    public class BasePayslipValidator<T> : AbstractValidator<T> where T : List<Payslip>
    {
        public BasePayslipValidator()
        {

        }
    }
}
