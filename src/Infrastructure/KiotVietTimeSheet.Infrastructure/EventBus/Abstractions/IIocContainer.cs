using System;

namespace KiotVietTimeSheet.Infrastructure.EventBus.Abstractions
{
    public interface IIocContainer
    {
        object Resolve(Type serviceType);
    }
}
