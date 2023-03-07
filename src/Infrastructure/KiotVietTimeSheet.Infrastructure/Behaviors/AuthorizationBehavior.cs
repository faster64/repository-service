using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Logging;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Application.Service.Interfaces;
using KiotVietTimeSheet.Domain.Utilities.Enums;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Utilities;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;
using RequiredPermissionAttribute = KiotVietTimeSheet.Application.Auth.Common.RequiredPermissionAttribute;
using RequiresAnyPermissionAttribute = KiotVietTimeSheet.Application.Auth.Common.RequiresAnyPermissionAttribute;

namespace KiotVietTimeSheet.Infrastructure.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;
        private readonly IMasterDbService _masterService;

        public AuthorizationBehavior(
            IAuthService authService,
            ILogger<AuthorizationBehavior<TRequest, TResponse>> logger,
            IMasterDbService masterService
        )
        {
            _authService = authService;
            _logger = logger;
            _masterService = masterService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                var isInternalRequest = request is IInternalRequest;
                if (isInternalRequest)
                {
                    return await next();
                }

                var isCommand = (request is BaseCommand) || (request is BaseCommand<TResponse>);
                // Kiểm trả thời hạn sử dụng
                if (isCommand)
                {
                    var tsSetting = await _masterService.GetRetailerAsync(_authService.Context.TenantId);
                    if (tsSetting.ExpiryDate == null || tsSetting.ExpiryDate != null && tsSetting.ExpiryDate.Value.Date < DateTime.Now.Date)
                    {
                        var  message = "Đã hết hạn sử dụng giải pháp Quản lý chấm công – Tính lương nhân viên. Liên hệ 1800 6162 (miễn cước) để được hỗ trợ.";
                        var ex = new KvTimeSheetFeatureExpiredException(new ErrorResult
                        {
                            Code = ErrorCode.ExpiriedDate.ToString(),
                            Message = message
                        });
                        var modelEx = GetMessageTemplateError(ex.Message);
                        _logger.LogError(ex, modelEx.ToJson());
                        throw ex;
                    }

                }
                // Nếu method k yêu cầu quyền sẽ kiểm tra class có yêu cầu quyền k
                var attributes = request.GetType().GetCustomAttributes(typeof(RequiredPermissionAttribute), false);
                if(!attributes.Any()) attributes = request.GetType().GetCustomAttributes(typeof(RequiresAnyPermissionAttribute), false);

                await Authenticate(attributes);

                return await next();
            }
            catch (Exception ex) when (ex is TargetInvocationException)
            {
                var baseException = ex.GetBaseException();
                var modelEx = GetMessageTemplateError(ex.Message);
                _logger.LogError(ex, modelEx.ToJson());
                throw new KvTimeSheetException(baseException.InnerException != null ? baseException.InnerException.Message : baseException.Message);
            }
        }

        private async Task Authenticate(object[] attributes)
        {
            if (!attributes.Any()) return;
            var permissionAttribute = new List<string>();
            var permissionAnyAttribute = new List<string>();
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(RequiredPermissionAttribute))
                {
                    permissionAttribute.AddRange(((RequiredPermissionAttribute)attribute).RequiredPermissions);
                }
                else
                {
                    permissionAnyAttribute.AddRange(((RequiresAnyPermissionAttribute)attribute).RequiredPermissions);
                }
            }
            if (permissionAttribute.Any())
            {
                await AuthenticateRequiredPermission(permissionAttribute.ToArray());
            }
            if (permissionAnyAttribute.Any())
            {
                await AuthenticateRequiredAnyPermission(permissionAnyAttribute.ToArray());
            }
        }

        private async Task AuthenticateRequiredPermission(string[] permissions)
        {
            if (await _authService.HasPermissions(permissions)) return;

            var ex = new KvTimeSheetUnAuthorizedException($"Người dùng {_authService.Context.User.UserName} không có quyền thực hiện");
            var modelEx = GetMessageTemplateError(ex.Message);
            _logger.LogError(ex, modelEx.ToJson());
            throw ex;
        }

        private async Task AuthenticateRequiredAnyPermission(string[] permissions)
        {
            if (await _authService.HasAnyPermission(permissions)) return;

            var ex = new KvTimeSheetUnAuthorizedException($"Người dùng {_authService.Context.User.UserName} không có quyền thực hiện");
            var modelEx = GetMessageTemplateError(ex.Message);
            _logger.LogError(ex, modelEx.ToJson());
            throw ex;
        }

        private MessageTemplateWriteLogger GetMessageTemplateError(string description)
        {
            var user = _authService.Context.User;
            var contextUser = _authService.Context;

            var modelTemplate = new MessageTemplateWriteLogger
            {
                UserId = user.Id,
                UserName = user.UserName,
                KvSessionId = user.KvSessionId,
                IndustryId = user.IndustryId,
                GroupId = user.GroupId,
                BranchId = contextUser.BranchId,
                TenantId = contextUser.TenantId,
                CurrentLang = contextUser.Language,
                RetailerCode = contextUser.TenantCode,
                UserIsAdmin = user.IsAdmin,
                Description = description
            };

            return modelTemplate;
        }
    }
}
