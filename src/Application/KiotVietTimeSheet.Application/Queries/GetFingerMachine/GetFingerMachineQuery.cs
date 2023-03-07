using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetFingerMachine
{
    public class GetFingerMachineQuery : QueryBase<PagingDataSource<FingerMachineDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetFingerMachineQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
