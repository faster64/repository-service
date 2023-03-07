using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Caching
{
    public interface ICacheClient : IDisposable
    {
        TimeSpan DefaultSlidingExpireTime { get; set; }

        TimeSpan? DefaultAbsoluteExpireTime { get; set; }

        #region Get Methods
        T Get<T>(string key, Func<string, T> factory) where T : class;

        T[] Get<T>(string[] keys, Func<string, T> factory) where T : class;

        Task<T> GetAsync<T>(string key, Func<string, Task<T>> factory) where T : class;

        Task<T[]> GetAsync<T>(string[] keys, Func<string, Task<T>> factoryAsync) where T : class;
        Task<T> GetAndSetWithExpireAsync<T>(string key, Func<string, Task<T>> factory, TimeSpan timeExpire) where T : class;

        T GetOrDefault<T>(string key) where T : class;

        T[] GetOrDefault<T>(string[] keys) where T : class;
        #endregion

        #region Set Methods
        void Set<T>(string key, T value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        void Set<T>(KeyValuePair<string, T>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);
        #endregion

        #region Remove Methods
        void Remove(string key);

        void Remove(string[] keys);

        void RemoveByParttern(string parttern);

        void RemoveByParttern(string[] partterns);
        #endregion
    }
}
