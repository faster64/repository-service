using System;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Service.Interfaces;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Service.Impls
{
    public class PosParamService : IPosParamService
    {
        private readonly IKiotVietServiceClient _kVServiceClient;
        private TimeSheetPosParam _timeSheetPosParam;
        private readonly ICacheClient _cacheClient;
        public PosParamService(
            IKiotVietServiceClient kVServiceClient,
            ICacheClient cacheClient
            )
        {
            _kVServiceClient = kVServiceClient;
            _cacheClient = cacheClient;
        }
        public async Task<TimeSheetPosParam> GetTimeSheetPosParam(int retailerId, string tenantCode)
        {
            try
            {
                using (var tokenSource = new CancellationTokenSource(Constant.MillisecondsDelay))
                {
                    if (_timeSheetPosParam != null) return _timeSheetPosParam;

                    var posparamKey = CacheKeys.GetTimeSheetPosParamCacheKey(retailerId);
                    var posParameterCache = _cacheClient.GetOrDefault<TimeSheetPosParam>(posparamKey);

                    if (posParameterCache != null) return posParameterCache;

                    var posParameter = await _kVServiceClient.GetTimeSheetPosParam(retailerId, tenantCode, tokenSource.Token);

                    if (posParameter != null)
                    {
                        _cacheClient.Set(posparamKey, posParameter, TimeSpan.FromMinutes(5));
                        _timeSheetPosParam = posParameter;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                throw HttpError.BadRequest(Constant.TaskCanceledExceptionMessage);
            }
            catch (Exception ex)
            {
                throw HttpError.Unauthorized(ex.Message);
            }
            return _timeSheetPosParam ?? new TimeSheetPosParam();
        }
    }
}
