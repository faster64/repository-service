using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Validations;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.PaysheetValidators
{
    public class CreateOrUpdatePaysheetValidator : BasePaysheetValidator<Paysheet>
    {
        #region PROPERTIES

        private readonly IPaysheetWriteOnlyRepository _paysheetWriteOnlyRepository;

        #endregion

        #region CONSTRUCTORS

        public CreateOrUpdatePaysheetValidator(
            IPaysheetWriteOnlyRepository paysheetWriteOnlyRepository,
            List<Payslip> payslips,
            PayslipPaymentDto firstPayslipPayment,
            bool isChangeCode = false,
            bool isCreate = false
        )
        {
            _paysheetWriteOnlyRepository = paysheetWriteOnlyRepository;
            ValidateCode();
            ValidateWorkingDayNumber();
            ValidateName();
            ValidateNote();
            ValidateCodeExist(isChangeCode);
        }

        #endregion

        #region PROTECTED METHODS

        protected void ValidateCodeExist(bool isChangeCode)
        {
            RuleFor(e => e)
                .MustAsync(async (paysheet, token) =>
                {
                    if (!string.IsNullOrEmpty(paysheet.Code) && (paysheet.Id == 0 || isChangeCode) && await _paysheetWriteOnlyRepository.AnyBySpecificationAsync(new CheckExistsCodeForOrmLiteSpec<Paysheet>(paysheet.Code)))
                    {
                        return false;
                    }
                    return true;
                })
                .WithMessage(Message.paysheet_existedCode);
        }

        protected void ValidateMinimumPayslip(List<Payslip> payslips, bool isCreate)
        {
            RuleFor(paysheet => paysheet)
                .Must(paysheet =>
                {
                    if (isCreate)
                    {
                        return payslips != null && payslips.Any();
                    }

                    return true;
                })
                .WithMessage(Message.paysheet_doNotHasAnyPayslip);
        }

        protected void ValidateMinimumPayslipPaymentTime(PayslipPaymentDto firstPayslipPayment)
        {
            RuleFor(paysheet => paysheet)
                .Must(paysheet =>
                {
                    if (paysheet.PaysheetCreatedDate > firstPayslipPayment.TransDate)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(Message.paysheet_minimumPayslipPaymentTime);
        }

        #endregion
    }
}
