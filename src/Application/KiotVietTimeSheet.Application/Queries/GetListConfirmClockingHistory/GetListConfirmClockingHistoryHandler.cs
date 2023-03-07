using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Utilities;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetListConfirmClockingHistory
{
    public class GetListConfirmClockingHistoryHandler : QueryHandlerBase,
        IRequestHandler<GetListConfirmClockingHistoryQuery, PagingDataSource<ConfirmClockingHistoryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IConfirmClockingHistoryReadOnlyRepository _confirmClockingHistoryReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IConfirmClockingReadOnlyRepository _confirmClockingReadOnly;
        private readonly IConfirmClockingDomainService _confirmClockingDomainService;
        public GetListConfirmClockingHistoryHandler(
            IAuthService authService,
            IConfirmClockingHistoryReadOnlyRepository confirmClockingHistoryReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IMapper mapper,
            IConfirmClockingReadOnlyRepository confirmClockingReadOnly,
            IConfirmClockingDomainService confirmClockingDomainService)
            : base(authService)
        {
            _mapper = mapper;
            _confirmClockingHistoryReadOnlyRepository = confirmClockingHistoryReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _confirmClockingReadOnly = confirmClockingReadOnly;
            _confirmClockingDomainService = confirmClockingDomainService;
        }
        public async Task<PagingDataSource<ConfirmClockingHistoryDto>> Handle(GetListConfirmClockingHistoryQuery request, CancellationToken cancellationToken)
        {
            var result = await _confirmClockingHistoryReadOnlyRepository.FiltersAsync(request.Query, request.IncludeSoftDelete);            
            var ret = _mapper.Map<PagingDataSource<ConfirmClockingHistory>, PagingDataSource<ConfirmClockingHistoryDto>>(result);
            var data = (ret.Data as List<ConfirmClockingHistoryDto>);
            var emloyeeIds = data.Select(x => x.ConfirmClocking.EmployeeId).ToList();
            var emloyees = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByIdsSpec(emloyeeIds), false, true);
            data.ForEach(x => {
                x.CheckTimeFormat = x.CreatedDate.ToString("HH:mm dd/MM/yyyy");
                x.EmployeeName = emloyees.FirstOrDefault(em => em.Id == x.ConfirmClocking.EmployeeId).Name;
            });
            await SetContentAndReason(data);
            return ret;
        }

        private async Task<List<ConfirmClockingHistoryDto>> SetContentAndReason(List<ConfirmClockingHistoryDto> histories)
        {
            var ids = histories.Select(x => x.ConfirmClockingId).ToList();
            var confirmClockings = await _confirmClockingReadOnly.GetBySpecificationAsync(new GetConfirmClokingByIdsSpec(ids));
            foreach (var history in histories)
            {
                var confirmClocking = confirmClockings.FirstOrDefault(x => x.Id == history.ConfirmClockingId);
                if (confirmClocking == null) continue;
                history.Content = _confirmClockingDomainService.GetContent(confirmClocking); 
                history.Reason = EnumHelpers.GetDisplayValue((ConfirmClockingType)confirmClocking.Type);
            }

            return histories;
        }
    }
}
