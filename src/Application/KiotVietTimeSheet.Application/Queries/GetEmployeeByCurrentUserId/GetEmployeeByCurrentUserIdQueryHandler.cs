using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeByCurrentUserId
{
    public class GetEmployeeByCurrentUserIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeByCurrentUserIdQuery, EmployeeDto>
    {
        private readonly IAuthService _authService;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        public GetEmployeeByCurrentUserIdQueryHandler(
            IAuthService authService,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
        ) : base(authService)
        {
            _authService = authService;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public async Task<EmployeeDto> Handle(GetEmployeeByCurrentUserIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _employeeReadOnlyRepository.GetByUserIdAsync(_authService.Context.User.Id, request.Reference, request.IncludeSoftDelete);
            return result.ConvertTo<EmployeeDto>();
        }
    }
}
