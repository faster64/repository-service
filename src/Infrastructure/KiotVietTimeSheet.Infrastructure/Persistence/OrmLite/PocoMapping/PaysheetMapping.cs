using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class PaysheetMapping
    {
        protected PaysheetMapping(){}
        public static void Mapping()
        {
            var model = typeof(Paysheet);
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("Payslips")
                .AddAttributes(new ReferenceAttribute());

            model.GetProperty("EmployeeTotal")
                .AddAttributes(new IgnoreAttribute());
            model.GetProperty("TotalNetSalary")
                .AddAttributes(new IgnoreAttribute());
            model.GetProperty("TotalNeedPay")
                .AddAttributes(new IgnoreAttribute());
            model.GetProperty("TotalPayment")
                .AddAttributes(new IgnoreAttribute());
            var dto = typeof(PaysheetDto);
            dto.GetProperty("Payslips")
                .AddAttributes(new ReferenceAttribute());
        }
    }
}
