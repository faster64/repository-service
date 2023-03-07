using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeAvailable
{
    public class GetEmployeeAvailableQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeAvailableQuery, PagingDataSource<EmployeeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        public GetEmployeeAvailableQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
        ) : base(authService)
        {
            _mapper = mapper;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public async Task<PagingDataSource<EmployeeDto>> Handle(GetEmployeeAvailableQuery request, CancellationToken cancellationToken)
        {
            var result = await _employeeReadOnlyRepository.GetAvailableEmployeesAsync(request.BranchId, request.WithoutId, request.Start, request.End, request.Skip, request.Take, request.Keyword);

            return _mapper.Map<PagingDataSource<Employee>, PagingDataSource<EmployeeDto>>(result);
        }
    }
}
