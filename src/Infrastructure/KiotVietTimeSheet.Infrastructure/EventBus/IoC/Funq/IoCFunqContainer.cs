using System;
using Funq;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.Infrastructure.EventBus.IoC.Funq
{
    public class IoCFunqContainer : IIocContainer
    {
        private readonly Container _container;
        public IoCFunqContainer(Container container)
        {
            _container = container;
        }

        public object Resolve(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }
    }
}
