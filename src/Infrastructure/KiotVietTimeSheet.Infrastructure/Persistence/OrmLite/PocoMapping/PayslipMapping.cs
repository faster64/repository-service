using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class PayslipMapping
    {
        protected PayslipMapping(){}
        public static void Mapping()
        {
            var model = typeof(Payslip);

            model.AddAttributes(new AliasAttribute("Payslip"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("PayslipDetails")
                .AddAttributes(new ReferenceAttribute());
            model.GetProperty("PayslipClockings")
                .AddAttributes(new ReferenceAttribute());
            model.GetProperty("PayslipClockingPenalizes")
                .AddAttributes(new ReferenceAttribute());
            model.GetProperty("PayslipPenalizes")
                .AddAttributes(new ReferenceAttribute());
        }
    }
}
