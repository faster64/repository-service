using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel.Types;
using KiotVietTimeSheet.Application.Logging;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Infrastructure.Securities.KvAuth;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Web;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    //[CheckActiveTimeSheetFilterAttribute]
    public class BaseAnonymousApi : Service
    {
        public ILogger Logger { get; }
        private readonly DomainNotificationHandler _notificationHandler;

        public BaseAnonymousApi(
            ILogger logger,
            INotificationHandler<DomainNotification> notificationHandler
        )
        {
            Logger = logger;
            _notificationHandler = (DomainNotificationHandler)notificationHandler;
            ConfigCulture();
        }

        public BaseAnonymousApi(
            ILogger logger
        )
        {
            Logger = logger;
            ConfigCulture();
        }

        protected IEnumerable<ErrorResult> Errors => _notificationHandler.Notifications.Select(n => n.ErrorResult);
        protected int CurrentRetailerId => SessionAs<KVSession>().CurrentRetailerId;
        protected int CurrentBranchId => SessionAs<KVSession>().CurrentBranchId;
        protected string CurrentLanguage => SessionAs<KVSession>().CurrentLang;
        public string CurrentRetailerCode => SessionAs<KVSession>().CurrentRetailerCode;

        public override Task<object> OnExceptionAsync(object requestDto, Exception ex)
        {
            var error = DtoUtils.CreateErrorResponse(requestDto, ex);
            if (!(error is IHttpError httpError)) return Task.FromResult(error);

            var exErr = GetMessageTemplateError(ex.Message);
            Logger.LogError(ex, exErr.ToJson());

            if (ex.GetType().ToString() == typeof(KvTimeSheetException).FullName)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            else 
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            return Task.FromResult(BadRequest(ex.Message, ex));
        }

        protected object BadRequest(string message, Exception ex)
        {
            ex.Data.Add("errors", ex);
            var exErr = GetMessageTemplateError(ex.Message);
            Logger.LogError(ex, exErr.ToJson());

            return new Response<object>
            {
                Message = message,
                Errors = ex
            };
        }

        protected Response<object> BadRequest(string message, IEnumerable<ErrorResult> errors)
        {
            var ex = new Exception(message);
            ex.Data.Add("errors", errors);
            var exErr = GetMessageTemplateError(ex.Message);
            Logger.LogError(ex, exErr.ToJson());

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return new Response<object>
            {
                Message = message,
                Errors = errors
            };
        }

        protected Response<object> CustomResponse(ErrorResult error)
        {
            Response.StatusCode = Convert.ToInt32(error.Code);
            return new Response<object>
            {
                Message = error.Message,
                Errors = error
            };
        }

        protected Response<T> Ok<T>(T obj) where T : class
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
            return new Response<T>
            {
                Message = string.Empty,
                Result = obj
            };
        }

        protected Response<object> Ok(string msg)
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
            return new Response<object>
            {
                Message = msg,
            };
        }

        protected Response<T> Ok<T>(string msg, T obj) where T : class
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
            return new Response<T>
            {
                Message = msg,
                Result = obj
            };
        }

        protected Response<object> Ok<T>(T obj, string msg) where T : struct
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
            return new Response<object>
            {
                Message = msg,
                Result = obj
            };
        }

        protected void ConfigCulture()
        {
            if (string.IsNullOrEmpty(CurrentLanguage)) return;
            var cultureInfo = new CultureInfo(CurrentLanguage);
            CultureInfo.CurrentUICulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            var info = CultureInfo.CreateSpecificCulture(CurrentLanguage);
            info.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            info.NumberFormat.CurrencyDecimalSeparator = ".";
            info.NumberFormat.CurrencyGroupSeparator = ",";
            info.NumberFormat.CurrencySymbol = string.Empty;
            info.NumberFormat.NumberDecimalSeparator = ".";
            info.NumberFormat.NumberGroupSeparator = ",";
            info.NumberFormat.PercentDecimalSeparator = ".";
            info.NumberFormat.PercentGroupSeparator = ",";
            Thread.CurrentThread.CurrentCulture = info;
            Thread.CurrentThread.CurrentUICulture = info;
        }

        private MessageTemplateWriteLogger GetMessageTemplateError(string description)
        {
            var modelTemplate = new MessageTemplateWriteLogger
            {
                BranchId = CurrentBranchId,
                TenantId = CurrentRetailerId,
                CurrentLang = CurrentLanguage,
                RetailerCode = CurrentRetailerCode,
                Description = description
            };

            return modelTemplate;
        }
    }
}
