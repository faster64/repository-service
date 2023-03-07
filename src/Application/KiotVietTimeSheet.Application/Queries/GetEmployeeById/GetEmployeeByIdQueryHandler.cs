using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeById
{
    public class GetEmployeeByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
    {
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;

        public GetEmployeeByIdQueryHandler(
            IAuthService authService,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
            ) : base(authService)
        {
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _employeeReadOnlyRepository.FindByIdAsync(request.Id, true, true);
            return result.ConvertTo<EmployeeDto>();
        }
    }
}
