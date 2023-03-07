using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Validations;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.PaysheetValidators
{
    public class HasCanceledPayslipValidator : BasePaysheetValidator<Paysheet>
    {
        public HasCanceledPayslipValidator(List<PayslipDto> payslipsDto)
        {
            ValidateCanceledPayslip(payslipsDto);
        }

        protected void ValidateCanceledPayslip(List<PayslipDto> payslipsDto)
        {
            RuleFor(p => p)
                .Custom((paySheet, context) =>
                {
                    var codePaySlips = 
                        paySheet.Payslips.Where(p => p.PayslipStatus == (byte) PayslipStatuses.Void)
                                         .Select(p => p.Code)
                                         .ToList();

                    var payslipDto = payslipsDto.FirstOrDefault(pd => paySheet.Payslips != null && codePaySlips.Any(p => p == pd.Code));

                    if (payslipDto != null)
                        context.AddFailure(string.Format(Message.paysheet_hasCanceledPayslip, payslipDto.Code));
                    
                });
        }
    }
}
