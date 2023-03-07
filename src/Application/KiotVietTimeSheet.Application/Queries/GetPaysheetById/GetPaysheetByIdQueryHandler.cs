using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using MediatR;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetById
{
    public class GetPaysheetByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPaysheetByIdQuery, PaysheetDto>
    {
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetPaysheetByIdQueryHandler(
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

        public async Task<PaysheetDto> Handle(GetPaysheetByIdQuery request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var paysheet = await _paysheetReadOnlyRepository.FindBySpecificationAsync(new FindPaysheetByIdSpec(id), true);
            var payslips = await _payslipReadOnlyRepository.GetBySpecificationAsync(new GetPayslipByPaysheetId(id), true);
            payslips = payslips.Where(x => !x.IsDeleted).ToList();
            paysheet.Merge(payslips);
            return _mapper.Map<PaysheetDto>(paysheet);
        }
    }
}
