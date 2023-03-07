using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetByIdForBgTask
{
    public class GetPaysheetByIdForBgTaskQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPaysheetByIdForBgTaskQuery, PaysheetDto>
    {
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetPaysheetByIdForBgTaskQueryHandler(
            IAuthService authService,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IMapper mapper

        ) : base(authService)
        {
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<PaysheetDto> Handle(GetPaysheetByIdForBgTaskQuery request, CancellationToken cancellationToken)
        {
            var paysheet = await _paysheetReadOnlyRepository.GetPaysheetById(request.Id);
            var payslips = await _payslipReadOnlyRepository.GetPayslipByPaysheetId(request.Id);
            if (payslips == null || !payslips.Any()) return _mapper.Map<PaysheetDto>(paysheet);
            paysheet.Merge(payslips);
            return _mapper.Map<PaysheetDto>(paysheet);
        }
    }
}
