using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.DeleteAllowance
{
    public class DeleteAllowanceCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteAllowanceCommand, Unit>
    {
        private readonly IAllowanceWriteOnlyRepository _allowanceWriteOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;


        public DeleteAllowanceCommandHandler(
            IEventDispatcher eventDispatcher,
            IAllowanceWriteOnlyRepository allowanceWriteOnlyRepository,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository
        ) : base(eventDispatcher)
        {
            _allowanceWriteOnlyRepository = allowanceWriteOnlyRepository;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
        }

        public async Task<Unit> Handle(DeleteAllowanceCommand request, CancellationToken cancellationToken)
        {
            long id = request.Id;
            var existingAllowanceById = await _allowanceReadOnlyRepository.FindByIdAsync(id);
            if (existingAllowanceById == null)
            {
                NotifyValidationErrors(typeof(Allowance), new List<string> { "Phụ cấp không tồn tại" });
                return Unit.Value;
            }

            existingAllowanceById.Delete();
            _allowanceWriteOnlyRepository.Delete(existingAllowanceById);
            await _allowanceWriteOnlyRepository.UnitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
