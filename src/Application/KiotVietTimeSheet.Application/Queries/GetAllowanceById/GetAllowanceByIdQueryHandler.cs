using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetAllowanceById
{
    public class GetAllowanceByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllowanceByIdQuery, AllowanceDto>
    {
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetAllowanceByIdQueryHandler(
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IMapper mapper,
            IAuthService authService
        ) : base(authService)
        {
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<AllowanceDto> Handle(GetAllowanceByIdQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<AllowanceDto>(await _allowanceReadOnlyRepository.FindByIdAsync(request.Id));
        }
    }
}
