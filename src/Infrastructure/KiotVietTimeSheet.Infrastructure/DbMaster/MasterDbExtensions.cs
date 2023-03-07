using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Infrastructure.DbMaster
{
    public static class MasterDbExtensions
    {
        public static void AddMasterDbService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMasterDbService, MasterDbService>();

            services.AddDbContextPool<DbMasterContext>(opts =>
            {
                opts.UseSqlServer(configuration.GetConnectionString("MasterDb"));
            });
        }
    }
}
