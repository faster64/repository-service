using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PayRateTemplateValidator;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Helpers;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdatePayrateTemplate
{
    public class UpdatePayrateTemplateCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdatePayrateTemplateCommand, PayRateFormDto>
    {
        private readonly IMapper _mapper;
        private readonly IPayRateTemplateReadOnlyRepository _payRateTemplateReadOnlyRepository;
        private readonly IPayRateTemplateWriteOnlyRepository _payRateTemplateWriteOnlyRepository;
        private readonly IPayRateReadOnlyRepository _payRateReadOnlyRepository;
        private readonly IPayRateWriteOnlyRepository _payRateWriteOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IEventDispatcher _eventDispatcher;

        public UpdatePayrateTemplateCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IPayRateTemplateReadOnlyRepository payRateTemplateReadOnlyRepository,
            IPayRateTemplateWriteOnlyRepository payRateTemplateWriteOnlyRepository,
            IPayRateReadOnlyRepository payRateReadOnlyRepository,
            IPayRateWriteOnlyRepository payRateWriteOnlyRepository,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _payRateTemplateReadOnlyRepository = payRateTemplateReadOnlyRepository;
            _payRateTemplateWriteOnlyRepository = payRateTemplateWriteOnlyRepository;
            _payRateReadOnlyRepository = payRateReadOnlyRepository;
            _payRateWriteOnlyRepository = payRateWriteOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<PayRateFormDto> Handle(UpdatePayrateTemplateCommand request, CancellationToken cancellationToken)
        {
            var payRateFormDto = request.PayRateTemplate;
            var updatePayRate = request.UpdatePayRate;
            var ruleList = SalaryRuleHelpers.GetRulesFromObjectByRuleValue(payRateFormDto);
            var existPayRateTemplateById = await _payRateTemplateReadOnlyRepository.FindByIdAsync(payRateFormDto.Id, true);
            if (existPayRateTemplateById == null)
            {
                NotifyPayRateTemplateInDbIsNotExists();
                return null;
            }
            var shiftFromMainSalaries = new List<Shift>();
            if (payRateFormDto.MainSalaryRuleValue?.MainSalaryValueDetails != null &&
                payRateFormDto.MainSalaryRuleValue.MainSalaryValueDetails.Any())
            {
                var shiftIdInMainSalary = payRateFormDto.MainSalaryRuleValue.MainSalaryValueDetails
                    .Where(x => x.ShiftId != 0).Select(x => x.ShiftId).ToList();

                shiftFromMainSalaries = await _shiftReadOnlyRepository.GetBySpecificationAsync(
                    new FindShiftByShiftIdsSpec(shiftIdInMainSalary), false, true);
            }

            existPayRateTemplateById.Update(payRateFormDto.Name, payRateFormDto.SalaryPeriod, ruleList, updatePayRate, request.IsGeneralSetting);

            var validateResultTemplate = await new CreateOrUpdatePayRateTemplateValidator(ruleList, _payRateTemplateWriteOnlyRepository, _allowanceReadOnlyRepository, _deductionReadOnlyRepository, shiftFromMainSalaries).ValidateAsync(existPayRateTemplateById);
            if (!validateResultTemplate.IsValid)
            {
                NotifyValidationErrors(typeof(PayRateTemplate), validateResultTemplate.Errors);
                return null;
            }

            await _payRateTemplateWriteOnlyRepository.UpdatePayRateTemplateAsync(existPayRateTemplateById);

            if (updatePayRate)
            {
                var referencePayRates = await _payRateReadOnlyRepository.GetBySpecificationAsync(new FindPayRateByTemplateIdSpecification(existPayRateTemplateById.Id), true);
                referencePayRates.ForEach(payRate =>
                {
                    var payRateDetailsUpdate = existPayRateTemplateById.PayRateTemplateDetails
                        .Select(td => new PayRateDetail(payRate.Id, td.RuleType, td.RuleValue, td.TenantId)).ToList();
                    if (!payRateDetailsUpdate.Any(x => x.RuleType == typeof(MainSalaryRule).Name))
                    {
                        var mainSalaryRule =
                            payRate.PayRateDetails?.FirstOrDefault(x => x.RuleType == typeof(MainSalaryRule).Name);
                        if (mainSalaryRule != null) payRateDetailsUpdate.Add(mainSalaryRule);
                    }
                    payRate.Update(payRateDetailsUpdate, existPayRateTemplateById.SalaryPeriod);
                });

                await _payRateWriteOnlyRepository.BatchUpdatePayRateAsync(referencePayRates);
            }

            await _payRateTemplateWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<PayRateFormDto>(existPayRateTemplateById);
        }

        private void NotifyPayRateTemplateInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(PayRateTemplate).Name, @"Mẫu lương không tồn tại"));
        }
    }
}
