using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Queries.GetEmployee
{
    public class GetEmployeeQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeQuery, PagingDataSource<EmployeeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IFingerPrintReadOnlyRepository _fingerPrintReadOnlyRepository;

        public GetEmployeeQueryHandler(
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IAuthService authService,
            IMapper mapper,
            IFingerPrintReadOnlyRepository fingerPrintReadOnlyRepository
            ) : base(authService)
        {
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _mapper = mapper;
            _fingerPrintReadOnlyRepository = fingerPrintReadOnlyRepository;
        }

        public async Task<PagingDataSource<EmployeeDto>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
        {
            var result = await _employeeReadOnlyRepository.FiltersAsync(request.Query, request.IncludeSoftDelete);
            var ret = _mapper.Map<PagingDataSource<Employee>, PagingDataSource<EmployeeDto>>(result);
            var employeeIds = ret.Data.Select(e => e.Id).ToList();

            // Lấy danh sách chấm công
            var listFingerPrint = request.IncludeFingerPrint
                ? await _fingerPrintReadOnlyRepository.GetBySpecificationAsync(
                    new FindFingerPrintByEmployeeIdsSpec(employeeIds))
                : new List<FingerPrint>();

            ret.Data.Each(e =>
            {
                e.FinqerCodes = listFingerPrint.Where(x => x.EmployeeId == e.Id).Select(x => x.FingerCode).ToList();
            });

            return ret;
        }
    }

    public class SyncEmployeeListQueryHandler:
        IRequestHandler<SyncEmployeeListQuery, PagingDataSource<SyncEmployeeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;

        public SyncEmployeeListQueryHandler(IMapper mapper, IEmployeeReadOnlyRepository employeeReadOnlyRepository
            )
        {
            _mapper = mapper;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public async Task<PagingDataSource<SyncEmployeeDto>> Handle(SyncEmployeeListQuery request, CancellationToken cancellationToken)
        {
            var employees = await _employeeReadOnlyRepository.GetEmployeeWithoutPermission(
                request.RetailerId,
                request.CurrentPage ?? 0, request.PageSize ?? Constant.SyncEmployeeDefaultPageSize, request.LastModifiedFrom);
            var result = new PagingDataSource<SyncEmployeeDto>()
            {
                Total = employees.Total,
                Data = new List<SyncEmployeeDto>()
            };
            foreach (var item in employees.Data)
            {
                result.Data.Add(_mapper.Map<SyncEmployeeDto>(item));
            }

            return result;
        }
    }
}
