using System;
using System.Collections.Generic;
using FluentValidation.Results;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Notification;

namespace KiotVietTimeSheet.Application.Abstractions
{
    public abstract class BaseCommandHandler
    {
        private readonly IEventDispatcher _eventDispatcher;

        protected BaseCommandHandler(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        protected void NotifyValidationErrors(Type type, IList<string> errors)
        {
            foreach (var error in errors)
            {
                _eventDispatcher.FireEvent(new DomainNotification(nameof(type), error));
            }
        }

        protected void NotifyValidationErrors(Type type, IList<ValidationFailure> errors)
        {
            foreach (var error in errors)
            {
                _eventDispatcher.FireEvent(new DomainNotification(nameof(type), new ErrorResult()
                {
                    Code = error.ErrorCode,
                    Message = error.ErrorMessage,
                }));
            }
        }
        protected void NotifyValidationError(Type type, string notify, string errorCode)
        {
            _eventDispatcher.FireEvent(new DomainNotification(nameof(type), new ErrorResult()
            {
                Code = errorCode,
                Message = notify,
            }));
        }
    }
}
