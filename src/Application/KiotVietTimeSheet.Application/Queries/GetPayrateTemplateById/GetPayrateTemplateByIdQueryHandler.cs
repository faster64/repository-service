using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPayrateTemplateById
{
    public class GetPayrateTemplateByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayrateTemplateByIdQuery, PayRateFormDto>
    {
        private readonly IPayRateTemplateReadOnlyRepository _payrateTemplateReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetPayrateTemplateByIdQueryHandler(
            IPayRateTemplateReadOnlyRepository payrateTemplateReadOnlyRepository,
            IMapper mapper,
            IAuthService authService
        ) : base(authService)
        {
            _payrateTemplateReadOnlyRepository = payrateTemplateReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<PayRateFormDto> Handle(GetPayrateTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<PayRateFormDto>(await _payrateTemplateReadOnlyRepository.FindByIdAsync(request.Id, true, true));
        }
    }
}
