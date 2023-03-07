using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.ShiftValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateShift
{
    public class CreateShiftCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateShiftCommand, ShiftDto>
    {
        private readonly IMapper _mapper;
        private readonly IShiftWriteOnlyRepository _shiftWriteOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;

        public CreateShiftCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IShiftWriteOnlyRepository shiftWriteOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _shiftWriteOnlyRepository = shiftWriteOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
        }

        public async Task<ShiftDto> Handle(CreateShiftCommand request, CancellationToken cancellationToken)
        {
            var addShift = new AddShift
            {
                Name = request.Shift.Name,
                From = request.Shift.From,
                To = request.Shift.To,
                IsActive = request.Shift.IsActive,
                BranchId = request.Shift.BranchId,
                CheckInBefore = request.Shift.CheckInBefore,
                CheckOutAfter = request.Shift.CheckOutAfter,
                IsGeneralSetting = request.IsGeneralSetting,
            };
            var shift = new Shift(addShift);

            var validateResult = await (new CreateOrUpdateShiftAsyncValidator(_shiftReadOnlyRepository, shift)).ValidateAsync(shift);

            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(Shift), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            _shiftWriteOnlyRepository.Add(shift);

            await _shiftWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<ShiftDto>(shift);
        }
    }
}
