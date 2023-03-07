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
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Utilities;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetListConfirmClocking
{
    public class GetListConfirmClockingHandler : QueryHandlerBase,
        IRequestHandler<GetListConfirmClockingQuery, PagingDataSource<ConfirmClockingDto>>
    {
        private readonly IMapper _mapper;
        private readonly IConfirmClockingReadOnlyRepository _confirmClockingReadOnlyRepository;
        private readonly IConfirmClockingDomainService _confirmClockingDomainService;
        public GetListConfirmClockingHandler(
            IAuthService authService,
            IConfirmClockingReadOnlyRepository confirmClockingReadOnlyRepository,
            IMapper mapper,
            IConfirmClockingDomainService confirmClockingDomainService)
            : base(authService)
        {
            _mapper = mapper;
            _confirmClockingReadOnlyRepository = confirmClockingReadOnlyRepository;
            _confirmClockingDomainService = confirmClockingDomainService;
        }
        public async Task<PagingDataSource<ConfirmClockingDto>> Handle(GetListConfirmClockingQuery request, CancellationToken cancellationToken)
        {
            var result = await _confirmClockingReadOnlyRepository.FiltersAsync(request.Query, request.IncludeSoftDelete);
            var ret = _mapper.Map<PagingDataSource<ConfirmClocking>, PagingDataSource<ConfirmClockingDto>>(result);
            (ret.Data as List<ConfirmClockingDto>).ForEach(x => { 
                x.CheckTimeFormat = x.CheckTime.ToString("HH:mm dd/MM/yyyy");
                x.Content = _confirmClockingDomainService.GetContent(result.Data.FirstOrDefault(y => y.Id == x.Id));
                x.Reason = EnumHelpers.GetDisplayValue((ConfirmClockingType)x.Type);
            });
            return ret;
        }
    }
}
