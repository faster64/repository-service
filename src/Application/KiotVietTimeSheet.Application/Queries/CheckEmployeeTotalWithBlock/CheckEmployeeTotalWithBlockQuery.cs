using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Queries.CheckEmployeeTotalWithBlock
{
    [Auth.Common.RequiredPermission(TimeSheetPermission.Employee_Update)]
    public class CheckEmployeeTotalWithBlockQuery : QueryBase<bool>
    {
        public int BlockUnit { get; protected set; }
        public int ContractType { get; protected set; }

        public CheckEmployeeTotalWithBlockQuery(int blockUnit, int contractType)
        {
            BlockUnit = blockUnit;
            ContractType = contractType;
        }
    }
}
