using System;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.Infrastructure.EventBus.IoC.NativeInjector
{
    public class IoCContainerNativeInjector : IIocContainer
    {
        private readonly IServiceProvider _serviceProvider;
        public IoCContainerNativeInjector(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object Resolve(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
    }
}
