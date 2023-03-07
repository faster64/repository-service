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
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Helpers;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreatePayrateTemplate
{
    public class CreatePayrateTemplateCommandHandler : BaseCommandHandler,
        IRequestHandler<CreatePayrateTemplateCommand, PayRateFormDto>
    {
        private readonly IMapper _mapper;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IPayRateTemplateWriteOnlyRepository _payRateTemplateWriteOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;

        public CreatePayrateTemplateCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IPayRateTemplateWriteOnlyRepository payRateTemplateWriteOnlyRepository,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IDeductionReadOnlyRepository deductionReadOnlyRepository
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _payRateTemplateWriteOnlyRepository = payRateTemplateWriteOnlyRepository;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
        }

        public async Task<PayRateFormDto> Handle(CreatePayrateTemplateCommand request, CancellationToken cancellationToken)
        {
            var payRateFormDto = request.PayRateTemplate;
            var rules = SalaryRuleHelpers.GetRulesFromObjectByRuleValue(payRateFormDto);
            var payRateTemplate = new PayRateTemplate(payRateFormDto.Name, payRateFormDto.SalaryPeriod, rules, request.BranchId, request.IsGeneralSetting);
            var shiftFromMainSalariesList = new List<Shift>();

            if (payRateFormDto.MainSalaryRuleValue?.MainSalaryValueDetails != null &&
                payRateFormDto.MainSalaryRuleValue.MainSalaryValueDetails.Any())
            {
                var shiftIdInMainSalary = payRateFormDto.MainSalaryRuleValue.MainSalaryValueDetails
                    .Where(x => x.ShiftId != 0).Select(x => x.ShiftId).ToList();

                shiftFromMainSalariesList = await _shiftReadOnlyRepository.GetBySpecificationAsync(
                    new FindShiftByShiftIdsSpec(shiftIdInMainSalary), false, true);
            }

            var templateValidateResult =
                await new CreateOrUpdatePayRateTemplateValidator(
                        rules,
                        _payRateTemplateWriteOnlyRepository,
                        _allowanceReadOnlyRepository,
                        _deductionReadOnlyRepository,
                        shiftFromMainSalariesList)
                    .ValidateAsync(payRateTemplate);
            if (!templateValidateResult.IsValid)
            {
                NotifyValidationErrors(typeof(PayRateTemplate), templateValidateResult.Errors);
                return null;
            }

            _payRateTemplateWriteOnlyRepository.Add(payRateTemplate);
            await _payRateTemplateWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<PayRateFormDto>(payRateTemplate);
        }

    }
}
