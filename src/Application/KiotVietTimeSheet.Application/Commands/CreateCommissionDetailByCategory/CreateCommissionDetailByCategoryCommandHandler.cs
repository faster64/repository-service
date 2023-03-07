using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByCategory
{
    public class CreateCommissionDetailByCategoryCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateCommissionDetailByCategoryCommand>
    {
        private readonly ICommissionDetailWriteOnlyRepository _commissionDetailWriteOnlyRepository;

        public CreateCommissionDetailByCategoryCommandHandler(
            IEventDispatcher eventDispatcher,
            ICommissionDetailWriteOnlyRepository commissionDetailWriteOnlyRepository
        )
            : base(eventDispatcher)
        {
            _commissionDetailWriteOnlyRepository = commissionDetailWriteOnlyRepository;
        }

        public async Task<Unit> Handle(CreateCommissionDetailByCategoryCommand request, CancellationToken cancellationToken)
        {
            var newCommissionDetails = request.CommissionDetails;
            _commissionDetailWriteOnlyRepository.BatchAdd(newCommissionDetails);
            await _commissionDetailWriteOnlyRepository.UnitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}

