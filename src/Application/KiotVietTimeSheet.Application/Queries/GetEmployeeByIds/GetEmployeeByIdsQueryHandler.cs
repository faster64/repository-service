using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeByIds
{
    public class GetEmployeeByIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeByIdsQuery, List<EmployeeDto>>
    {
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetEmployeeByIdsQueryHandler(
            IAuthService authService,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IMapper mapper
            ) : base(authService)
        {
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeDto>> Handle(GetEmployeeByIdsQuery request, CancellationToken cancellationToken)
        {
            var result = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByIdsSpec(request.Ids), false, true);
            return _mapper.Map<List<EmployeeDto>>(result);
        }
    }
}
