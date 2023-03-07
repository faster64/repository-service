using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class JobTitleMapping
    {
        protected JobTitleMapping(){}
        public static void Mapping()
        {
            var model = typeof(JobTitle);

            model.AddAttributes(new AliasAttribute("JobTitle"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
        }
    }
}
