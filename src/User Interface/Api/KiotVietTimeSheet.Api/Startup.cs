using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using KiotVietTimeSheet.Api.Configuration;
using KiotVietTimeSheet.Api.Extensions;
using KiotVietTimeSheet.Api.IoC.NativeInjector;
using KiotVietTimeSheet.Infrastructure.Configuration;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping;
using Microsoft.AspNetCore.ResponseCompression;
using KiotVietTimeSheet.Api.Middlewares;
using KiotVietTimeSheet.Infrastructure.DbMaster;

namespace KiotVietTimeSheet.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var infrastructureConfiguration = new InfrastructureConfiguration(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(c => new HttpClient());

            services.AddCors(o => o.AddPolicy("Policy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddAutoMapper();

            Application.AutoMapperConfigurations.AutoMapping.RegisterMappings();

            RegisterServices(services);

            services.AddMediatR(typeof(Startup));

            services.AddMediatorBehaviors();

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddCustomDbContext();

            services.AddMasterDbService(Configuration);

            services.AddEventBus();

            services.AddCommands();

            services.AddQueries();

            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddReadOnlyRepositories();

            services.AddRedisMq();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiConfiguration apiConfiguration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseResponseCompression();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append(apiConfiguration.AccessControlAllowOrigin, "*");
                    ctx.Context.Response.Headers.Append(apiConfiguration.AccessControlAllowHeaders,
                        apiConfiguration.AccessControlAllowHeadersValue);
                },
                FileProvider =
                    new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                        apiConfiguration.RootFolderName)),
                RequestPath = new PathString("")
            });

            app.UseMiddleware<RequestLogContextMiddleware>();

            app.UseCors("Policy");

            app.UseServiceStack(new AppHost
            {
                AppSettings = new NetCoreAppSettings(Configuration)
            });

            app.UseMvc();

            PocoMapping.Factory();
        }

        private void RegisterServices(IServiceCollection services)
        {
            NativeInjectorBootStrapper.Register(services, Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
