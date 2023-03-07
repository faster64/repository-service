using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System;

namespace KiotVietTimeSheet.Infrastructure.EventBus.RabbitMQ
{
    public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>, IDisposable
    {
        private readonly IConnection _connection;

        public RabbitModelPooledObjectPolicy(IConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.CreateConnection();
        }

        public IModel Create()
        {
            return _connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            else
            {
                obj?.Dispose();
                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool dispose)
        {
            if (dispose)
            {
                _connection.Dispose();
            }
        }
    }
}
