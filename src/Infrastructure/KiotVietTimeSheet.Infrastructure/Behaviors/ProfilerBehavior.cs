using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Infrastructure.Configuration;
using MediatR;

namespace KiotVietTimeSheet.Infrastructure.Behaviors
{
    public class ProfilerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        #region Properties
        private readonly ILogger<ProfilerBehavior<TRequest, TResponse>> _logger;
        #endregion

        public ProfilerBehavior(
            ILogger<ProfilerBehavior<TRequest, TResponse>> logger
        )
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var stopWatch = Stopwatch.StartNew();
            try
            {
                var response = await next();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            finally
            {
                stopWatch.Stop();
                if (InfrastructureConfiguration.UseProfiler)
                {
                    _logger.LogInformation($"Execution time of command/query {request.GetType().Name}: {stopWatch.ElapsedMilliseconds} ms");
                }
            }
        }
    }
}
