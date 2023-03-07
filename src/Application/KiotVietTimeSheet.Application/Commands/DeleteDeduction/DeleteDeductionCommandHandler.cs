using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.DeleteDeduction
{
    public class DeleteDeductionCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteDeductionCommand, Unit>
    {
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly IDeductionWriteOnlyRepository _deductionWriteOnlyRepository;
        public DeleteDeductionCommandHandler(
            IEventDispatcher eventDispatcher,
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IDeductionWriteOnlyRepository deductionWriteOnlyRepository
        ) : base(eventDispatcher)
        {
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
            _deductionWriteOnlyRepository = deductionWriteOnlyRepository;
        }
        public async Task<Unit> Handle(DeleteDeductionCommand request, CancellationToken cancellationToken)
        {
            long id = request.Id;
            var existingDeductionById = await _deductionReadOnlyRepository.FindByIdAsync(id);
            if (existingDeductionById == null)
            {
                NotifyValidationErrors(typeof(Deduction), new List<string> { "Giảm trừ không tồn tại" });
                return Unit.Value;
            }

            existingDeductionById.Delete();
            _deductionWriteOnlyRepository.Delete(existingDeductionById);
            await _deductionWriteOnlyRepository.UnitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
