using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Infrastructure.Configuration;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Infrastructure.Proxies
{
    public class ProfilerProxy<T> : BaseProxy
    {
        #region Properties
        private T _decorated;
        private ILogger<ProfilerProxy<T>> _logger;
        private ILogger<ProfilerProxy<T>> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<ProfilerProxy<T>>>());
        #endregion

        public static T Create(
            T decorated,
            ILogger<ProfilerProxy<T>> logger
        )
        {
            object proxy = Create<T, ProfilerProxy<T>>();
            ((ProfilerProxy<T>)proxy).SetParameters(decorated, logger);
            return (T)proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var stopWatch = Stopwatch.StartNew();
            try
            {
                if (!InfrastructureConfiguration.UseProfiler) return InternalInvoke(_decorated, targetMethod, args);
                var result = InternalInvoke(_decorated, targetMethod, args);
                return result;
            }
            catch (Exception ex) when (ex is TargetInvocationException)
            {
                if (ex.InnerException == null || ex.InnerException.GetType().GetProperty(nameof(ErrorResult)) == null)
                    throw new KvTimeSheetException(ex.InnerException != null ? ex.InnerException.Message : ex.Message);

                var errorResult = ex.InnerException.GetType().GetProperty(nameof(ErrorResult))?.GetValue(ex.InnerException, null);
                if (errorResult != null)
                {
                    throw new KvTimeSheetFeatureExpiredException((ErrorResult)errorResult);
                }

                throw;
            }
            finally
            {
                stopWatch.Stop();
                if (InfrastructureConfiguration.UseProfiler)
                {
                    Logger.LogInformation(
                        $"Execution time of interface {targetMethod.GetDeclaringTypeName()} method {targetMethod.Name}: {stopWatch.ElapsedMilliseconds} ms");
                }
            }
        }


        #region Private methods
        private void SetParameters(T decorated, ILogger<ProfilerProxy<T>> logger)
        {
            if (decorated == null) throw new ArgumentException(nameof(decorated));
            _decorated = decorated;
            _logger = logger;
        }
        #endregion
    }
}
