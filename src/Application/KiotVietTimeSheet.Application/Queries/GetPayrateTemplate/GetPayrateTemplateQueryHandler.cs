using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPayrateTemplate
{
    public class GetPayrateTemplateQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayrateTemplateQuery, PagingDataSource<PayRateFormDto>>
    {
        private readonly IPayRateTemplateReadOnlyRepository _payrateTemplateReadOnlyRepository;
        private readonly IPayRateReadOnlyRepository _payrateReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetPayrateTemplateQueryHandler(
            IPayRateTemplateReadOnlyRepository payrateTemplateReadOnlyRepository,
            IPayRateReadOnlyRepository payrateReadOnlyRepository,
            IMapper mapper,
            IAuthService authService
        ) : base(authService)
        {
            _payrateTemplateReadOnlyRepository = payrateTemplateReadOnlyRepository;
            _payrateReadOnlyRepository = payrateReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<PagingDataSource<PayRateFormDto>> Handle(GetPayrateTemplateQuery request, CancellationToken cancellationToken)
        {
            var result = await _payrateTemplateReadOnlyRepository.FiltersAsync(request.Query);
            var payrateTemplateIdList = result.Data.Select(x => x.Id).ToList();
            var payrateList = await _payrateReadOnlyRepository.FiltersAsync(payrateTemplateIdList);
            var resultDto = _mapper.Map<PagingDataSource<PayRateFormDto>>(result);
            if (payrateList != null && payrateList.Count > 0)
            {
                var templatePayrateDtoList = payrateList.GroupBy(x => x.PayRateTemplateId)
                                                    .Select(gr => new
                                                    {
                                                        PayRateTemplateId = gr.Key,
                                                        TotalEmployee = gr.Count()
                                                    })
                                                    .ToList();

                foreach (var item in resultDto.Data)
                {
                    var existedPayrate = templatePayrateDtoList.FirstOrDefault(x => x.PayRateTemplateId == item.Id);
                    item.TotalEmployee = existedPayrate != null ? existedPayrate.TotalEmployee : 0;
                }
            }
            return resultDto;

        }
    }
}
