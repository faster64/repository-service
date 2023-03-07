using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Infrastructure.Proxies
{
    public abstract class BaseProxy : DispatchProxy
    {
        protected object InternalInvoke<T>(T decorated, MethodBase targetMethod, object[] args)
        {
            var result = targetMethod.Invoke(decorated, args);

            if (!(result is Task resultTask)) return result;

            if (resultTask.Exception != null)
            {
                throw resultTask.Exception;
            }

            var property = resultTask.GetType().GetTypeInfo().GetProperties()
                .FirstOrDefault(p => p.Name == "Result");
            if (property != null)
            {
                property.GetValue(resultTask);
            }
            return result;
        }
    }
}
