using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Queries.GetShiftById
{
    public class GetShiftByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetShiftByIdQuery, ShiftDto>
    {
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        public GetShiftByIdQueryHandler(
            IAuthService authService,
            IShiftReadOnlyRepository shiftReadOnlyRepository
        ) : base(authService)
        {
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
        }

        public async Task<ShiftDto> Handle(GetShiftByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _shiftReadOnlyRepository.FindByIdAsync(request.Id);
            return result.ConvertTo<ShiftDto>();
        }
    }
}
