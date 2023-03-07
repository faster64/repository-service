using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeByUserId
{
    public class GetEmployeeByUserIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeByUserIdQuery, EmployeeDto>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        public GetEmployeeByUserIdQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
        ) : base(authService)
        {
            _mapper = mapper;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public async Task<EmployeeDto> Handle(GetEmployeeByUserIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _employeeReadOnlyRepository.FiltersAsync(request.Query);
            return _mapper.Map<EmployeeDto>(result.Data.FirstOrDefault());
        }
    }
}
