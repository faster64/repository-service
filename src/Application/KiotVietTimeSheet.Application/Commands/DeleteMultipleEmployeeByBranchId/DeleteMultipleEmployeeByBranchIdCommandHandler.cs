using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Commands.DeleteMultipleEmployee;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Application.Commands.DeleteMultipleEmployeeByBranchId
{
    public class DeleteMultipleEmployeeByBranchIdCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteMultipleEmployeeByBranchIdCommand, Unit>
    {
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IDeleteEmployeeDomainService _deleteEmployeeDomainService;
        private readonly ILogger<DeleteMultipleEmployeeByBranchIdCommandHandler> _logger;
        public DeleteMultipleEmployeeByBranchIdCommandHandler(
            IEventDispatcher eventDispatcher,
            ILogger<DeleteMultipleEmployeeByBranchIdCommandHandler> logger,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IDeleteEmployeeDomainService deleteEmployeeDomainService
            )
            : base(eventDispatcher)
        {
            _logger = logger;
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _deleteEmployeeDomainService = deleteEmployeeDomainService;
        }

        public async Task<Unit> Handle(DeleteMultipleEmployeeByBranchIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.BranchId <= 0) return Unit.Value;
                var employees = await _employeeWriteOnlyRepository.GetBySpecificationAsync(new FindEmployeeByBranchIdSpec((int)request.BranchId));
                if (employees == null || !employees.Any()) return Unit.Value;

                var result = await _deleteEmployeeDomainService.DeleteEmployees(new DomainService.Dto.DeleteEmployeesDomainServiceDto { Ids = employees.Select(x => x.Id).ToList() });
                if (!result)
                    return Unit.Value;

                await _employeeWriteOnlyRepository.UnitOfWork.CommitAsync();
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Unit.Value;
            }
        }
    }
}
