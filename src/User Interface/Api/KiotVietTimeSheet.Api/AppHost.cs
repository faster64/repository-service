using System.Net;
using Funq;
using KiotVietTimeSheet.Api.Configuration;
using ServiceStack;
using ServiceStack.Api.Swagger;
using KiotVietTimeSheet.Api.ServiceInterface;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Runtime.Exception;
using ServiceStack.Validation;
using ServiceStack.Text;
using ServiceStack.Auth;
using KiotVietTimeSheet.Utilities;
using KiotVietTimeSheet.Infrastructure.Securities.KvAuth;
using KiotVietTimeSheet.SharedKernel.Auth;

namespace KiotVietTimeSheet.Api
{
    public class AppHost : AppHostBase
    {
        public AppHost()
            : base("KiotViet TimeSheet API",
                  typeof(PingApi).Assembly
            )
        { }

        public override void Configure(Container container)
        {
            // Configuration servicestack plugins
            ConfigPlugins();

            // Configuration json serializer
            ConfigJsonSerializer();

            ConfigExceptionHandler();

            var logger = HostContext.TryResolve<ILogger<AppHost>>();
            logger.LogInformation($"KiotViet TimeSheet API starting...");

            Globals.SetBuildVersion();
            logger.LogInformation($"Build version: {Globals.BuildVersion}");
        }

        /// <summary>
        /// Configuration and register servicestack plugins
        /// </summary>
        private void ConfigPlugins()
        {
            var apiConfiguration = HostContext.Resolve<IApiConfiguration>();
            if (apiConfiguration.EnableSwaggerFeature) Plugins.Add(new SwaggerFeature());
            Plugins.Add(new ValidationFeature() { });
            Plugins.Add(new CorsFeature() { });
            Plugins.Add(ConfigAutoQueryData());
            Plugins.Add(
                new AuthFeature(
                    () => new KVSession(),
                    new IAuthProvider[]
                    {
                        new JwtAuthProvider(AppSettings)
                        {
                            ValidateToken = (js,req) => req.GetJwtToken().LastRightPart('.').FromBase64UrlSafe().Length >= 32,                            
                            PopulateSessionFilter = (session, o, req) =>
                            {
                                var sess = session as KVSession;
                                if (sess == null) return;

                                sess.CurrentRetailerCode = o["kvrcode"];
                                sess.KvSessionId = o["kvses"];
                                sess.CurrentRetailerId = ConvertHelper.ToInt32(o["kvrid"]);
                                sess.GroupId = ConvertHelper.ToInt32(o["kvrgid"]);
                                sess.CurrentIndustryId  =ConvertHelper.ToInt32(o["kvrindid"]);
                                var uid = ConvertHelper.ToInt64(o["kvuid"]);
                                var uname = o["preferred_username"];
                                sess.CurrentBranchId = ConvertHelper.ToInt32(req.Headers["BranchId"]);
                                sess.CurrentLang = "vi-VN";
                                sess.HttpMethod = req.Verb;
                                sess.CurrentUser = new SessionUser
                                {
                                    Id = uid,
                                    UserName = uname,
                                    GivenName = o["kvugvname"],
                                    RetailerId = ConvertHelper.ToInt32(o["kvurid"]),
                                    Type = ConvertHelper.ToByte(o["kvutype"]),
                                    IsLimitTime = ConvertHelper.ToBoolean(o["kvulimit"]),
                                    IsAdmin =  ConvertHelper.ToBoolean(o["kvuadmin"]),
                                    IsActive = ConvertHelper.ToBoolean(o["kvuact"]),
                                    IsLimitedByTrans = ConvertHelper.ToBoolean(o["kvulimittrans"]),
                                    IsShowSumRow = ConvertHelper.ToBoolean(o["kvushowsum"]),
                                    GroupId = ConvertHelper.ToInt32(o["kvrgid"]),
                                    KvSessionId = o["kvses"],
                                    IndustryId = ConvertHelper.ToInt32(o["kvrindid"]),
                                    Language = "vi-VN"
                                };

                                if (o.ContainsKey("clockinggps"))
                                {
                                    sess.CurrentUser.SessionClockingGps = o["clockinggps"].ConvertTo<SessionClockingGps>();
                                }
                            }
                        },
                    }));

            SetConfig(new HostConfig
            {
                MapExceptionToStatusCode =
                {
                    { typeof(KvTimeSheetModuleUnActiveException), 400 }
                },
                DefaultContentType = MimeTypes.Json
            });
        }

        private static void ConfigJsonSerializer()
        {
            JsConfig.ExcludeTypeInfo = true;
            JsConfig.DateHandler = DateHandler.ISO8601;
            JsConfig.TreatEnumAsInteger = true;
        }

        private static AutoQueryFeature ConfigAutoQueryData()
        {
            var aq = new AutoQueryFeature();
            aq.ImplicitConventions.Add("%neq", aq.ImplicitConventions["%NotEqualTo"]);
            aq.ImplicitConventions.Add("%eq", "{Field} = {Value}");
            aq.ImplicitConventions.Add("%contains", "{Field} Like N'%{Value}%'");

            return aq;
        }

        private void ConfigExceptionHandler()
        {
            UncaughtExceptionHandlers.Add(async (req, res, operationName, ex) =>
            {
                var logger = HostContext.TryResolve<ILogger<AppHost>>();
                var baseException = ex.GetBaseException();
                var isTimeSheetUnActiveException = baseException is KvTimeSheetModuleUnActiveException;
                var isUnauthorizedException = baseException is HttpError error && error.Status == (int)HttpStatusCode.Unauthorized;

                logger?.LogError(ex, $"{ex.Message} - InnerException: {ex.GetBaseException()?.Message}");

                if (isUnauthorizedException)
                {
                    await res.WriteErrorToResponse(
                        httpReq: req,
                        contentType: "application/json; charset=utf-8",
                        operationName: operationName,
                        errorMessage: baseException?.Message,
                        ex: baseException,
                        statusCode: (int)HttpStatusCode.Unauthorized);
                }

                if (isTimeSheetUnActiveException)
                {
                    await res.WriteErrorToResponse(
                        httpReq: req,
                        contentType: "application/json; charset=utf-8",
                        operationName: operationName,
                        errorMessage: baseException?.Message,
                        ex: baseException,
                        statusCode: (int)HttpStatusCode.BadRequest);
                }

            });
        }
    }
}
