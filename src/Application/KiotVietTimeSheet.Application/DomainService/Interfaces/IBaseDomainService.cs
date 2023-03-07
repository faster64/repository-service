using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface IBaseDomainService
    {
        void NotifyValidationErrors(Type type, IList<string> errors);
        void NotifyValidationErrors(Type type, IList<ValidationFailure> errors);
    }
}
