using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PayslipValidator;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class CreateOrUpdatePayslipDomainService : ICreateOrUpdatePayslipDomainService
    {
        #region Properties
        private readonly IPayslipWriteOnlyRepository _payslipWriteOnlyRepository;
        private readonly IPayslipClockingWriteOnlyRepository _payslipClockingWriteOnlyRepository;
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        #endregion

        #region Constructors
        public CreateOrUpdatePayslipDomainService(
            IPayslipWriteOnlyRepository payslipWriteOnlyRepository,
            IPayslipClockingWriteOnlyRepository payslipClockingWriteOnlyRepository,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
            )
        {
            _payslipWriteOnlyRepository = payslipWriteOnlyRepository;
            _payslipClockingWriteOnlyRepository = payslipClockingWriteOnlyRepository;
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }
        #endregion

        #region Public methods
        public async Task<PayslipDomainServiceDto> BatchCreateAsync(List<Payslip> payslips, DateTime startTime, DateTime endTime)
        {
            var payslipValidate = new CreateOrUpdatePayslipValidator(_payslipReadOnlyRepository, _employeeReadOnlyRepository).Validate(payslips);
            var result = new PayslipDomainServiceDto
            {
                IsValid = true,
                ValidationErrors = new List<string>()
            };

            if (!payslipValidate.IsValid)
            {
                result.IsValid = false;
                result.ValidationErrors = payslipValidate.Errors.Select(e => e.ErrorMessage).ToList();
                return result;
            }

            _payslipWriteOnlyRepository.BatchAdd(payslips);
            await _payslipClockingWriteOnlyRepository.CreateOrUpdateAsync(payslips, startTime, endTime);
            return result;
        }
        #endregion

        #region Private methods

        #endregion
    }
}
