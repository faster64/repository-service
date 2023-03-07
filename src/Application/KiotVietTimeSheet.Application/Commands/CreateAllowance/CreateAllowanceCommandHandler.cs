using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PayRateValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using ServiceStack;
using ServiceStack.Text;

namespace KiotVietTimeSheet.Application.Commands.CreateAllowance
{
    public class CreateAllowanceCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateAllowanceCommand, AllowanceDto>
    {
        private readonly IMapper _mapper;
        private readonly AllowanceCreateOrUpdateValidator _allowanceCreateOrUpdateValidator;
        private readonly IAllowanceWriteOnlyRepository _allowanceWriteOnlyRepository;

        public CreateAllowanceCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IAllowanceWriteOnlyRepository allowanceWriteOnlyRepository,
            AllowanceCreateOrUpdateValidator allowanceCreateOrUpdateValidator
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _allowanceCreateOrUpdateValidator = allowanceCreateOrUpdateValidator;
            _allowanceWriteOnlyRepository = allowanceWriteOnlyRepository;
        }

        public async Task<AllowanceDto> Handle(CreateAllowanceCommand request, CancellationToken cancellationToken)
        {
            var allowanceDto = request.Allowance;
            Allowance allowance = allowanceDto.ConvertTo<Allowance>();

            var validator = await _allowanceCreateOrUpdateValidator.ValidateAsync(allowance);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Allowance), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return allowanceDto;
            }
            _allowanceWriteOnlyRepository.Add(allowance);
            await _allowanceWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<AllowanceDto>(allowance);
        }
    }
}
