using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDataTrial
{
    public class CreateCommissionDataTrialCommand : BaseCommand<CommissionDto>, IInternalRequest
    {
        public CommissionDto Commission { get; }
        public CreateCommissionDataTrialCommand(CommissionDto commission)
        {
            Commission = commission;
        }
    }
}
