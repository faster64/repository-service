using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetsOldVersionByIds
{
    public class GetPaysheetsOldVersionByIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPaysheetsOldVersionByIdsQuery, List<PaysheetDto>>
    {
        private readonly IMapper _mapper;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;

        public GetPaysheetsOldVersionByIdsQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository

        ) : base(authService)
        {
            _mapper = mapper;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
        }

        public async Task<List<PaysheetDto>> Handle(GetPaysheetsOldVersionByIdsQuery request, CancellationToken cancellationToken)
        {
            object[] paysheetIds = { request.Ids };
            return _mapper.Map<List<PaysheetDto>>(await _paysheetReadOnlyRepository.FindByIdsAsync(paysheetIds));
        }
    }
}
