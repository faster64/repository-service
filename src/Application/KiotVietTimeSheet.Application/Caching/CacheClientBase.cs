using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Caching
{
    public abstract class CacheClientBase : ICacheClient
    {
        private readonly ILogger<CacheClientBase> _logger;
        private readonly object SyncObj = new object();
        private readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1);

        public TimeSpan DefaultSlidingExpireTime { get; set; }
        public TimeSpan? DefaultAbsoluteExpireTime { get; set; }

        protected CacheClientBase(ILogger<CacheClientBase> logger)
        {
            DefaultSlidingExpireTime = TimeSpan.FromHours(1);
            DefaultAbsoluteExpireTime = TimeSpan.FromHours(1);
            _logger = logger;
        }

        public virtual T Get<T>(string key, Func<string, T> factory) where T : class
        {
            T item = null;

            try
            {
                item = GetOrDefault<T>(key);
            }
            catch (Exception exForGetOrDefault)
            {
                _logger.LogError(exForGetOrDefault, exForGetOrDefault.ToString());
            }

            if (item != null) return item;
            lock (SyncObj)
            {
                try
                {
                    item = GetOrDefault<T>(key);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.ToString());
                }

                if (item != null) return item;
                item = factory(key);

                if (item == null)
                {
                    return null;
                }

                try
                {
                    Set(key, item);
                }
                catch (Exception exE)
                {
                    _logger.LogError(exE, exE.ToString());
                }
            }

            return item;
        }
        public virtual T[] Get<T>(string[] keys, Func<string, T> factory) where T : class
        {
            T[] items = null;

            try
            {
                items = GetOrDefault<T>(keys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
            }

            if (items == null)
            {
                items = new T[keys.Length];
            }

            if (items.All(i => i != null)) return items;
            lock (SyncObj)
            {
                try
                {
                    items = GetOrDefault<T>(keys);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.ToString());
                }

                var fetched = new List<KeyValuePair<string, T>>();
                for (var i = 0; i < items.Length; i++)
                {
                    var key = keys[i];
                    var value = items[i] ?? factory(key);

                    if (value == null) continue;
                    fetched.Add(new KeyValuePair<string, T>(key, value));
                }

                if (!fetched.Any()) return items;
                try
                {
                    Set(fetched.ToArray());
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.ToString());
                }
            }

            return items;
        }
        public virtual async Task<T> GetAndSetWithExpireAsync<T>(string key, Func<string, Task<T>> factory, TimeSpan timeExpire) where T : class
        {
            T item = null;

            try
            {
                item = await GetOrDefaultAsync<T>(key);
            }
            catch (Exception exAsync)
            {
                _logger.LogError(exAsync, exAsync.ToString());
            }

            if (item != null) return item;
            // lock
            await _asyncLock.WaitAsync();
            try
            {
                item = await GetOrDefaultAsync<T>(key);

                if (item == null && factory!=null)
                {
                    item = await factory(key);

                    if (item == null)
                    {
                        // unlock
                        _asyncLock.Release();
                        return null;
                    }

                    await SetAsync(key, item, timeExpire);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
            }

            // unlock
            _asyncLock.Release();

            return item;
        }

        public virtual async Task<T> GetAsync<T>(string key, Func<string, Task<T>> factory) where T : class
        {
            T item = null;

            try
            {
                item = await GetOrDefaultAsync<T>(key);
            }
            catch (Exception exAsync)
            {
                _logger.LogError(exAsync, exAsync.ToString());
            }

            if (item != null) return item;
            // lock
            await _asyncLock.WaitAsync();
            try
            {
                item = await GetOrDefaultAsync<T>(key);

                if (item == null)
                {
                    item = await factory(key);

                    if (item == null)
                    {
                        // unlock
                        _asyncLock.Release();
                        return null;
                    }

                    await SetAsync(key, item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
            }

            // unlock
            _asyncLock.Release();

            return item;
        }
        public virtual async Task<T[]> GetAsync<T>(string[] keys, Func<string, Task<T>> factoryAsync) where T : class
        {
            T[] items = null;

            try
            {
                items = await GetOrDefaultAsync<T>(keys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
            }

            if (items == null)
            {
                items = new T[keys.Length];
            }

            if (items.All(i => i != null)) return items;
            // lock
            await _asyncLock.WaitAsync();
            try
            {
                items = await GetOrDefaultAsync<T>(keys);

                var fetched = new List<KeyValuePair<string, T>>();
                for (var i = 0; i < items.Length; i++)
                {
                    var key = keys[i];
                    var value = items[i] ?? await factoryAsync(key);

                    if (value != null)
                    {
                        fetched.Add(new KeyValuePair<string, T>(key, value));
                    }
                }

                if (fetched.Any())
                {
                    await SetAsync(fetched.ToArray());
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.ToString());
            }

            // unlock
            _asyncLock.Release();

            return items;
        }

        public abstract T GetOrDefault<T>(string key) where T : class;

        public virtual T[] GetOrDefault<T>(string[] keys) where T : class
        {
            return keys.Select(GetOrDefault<T>).ToArray();
        }

        public virtual Task<T> GetOrDefaultAsync<T>(string key) where T : class
        {
            return Task.FromResult(GetOrDefault<T>(key));
        }

        public virtual Task<T[]> GetOrDefaultAsync<T>(string[] keys) where T : class
        {
            return Task.FromResult(GetOrDefault<T>(keys));
        }

        public abstract void Set<T>(string key, T value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        public virtual void Set<T>(KeyValuePair<string, T>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            foreach (var pair in pairs)
            {
                Set(pair.Key, pair.Value, slidingExpireTime, absoluteExpireTime);
            }
        }

        public virtual Task SetAsync<T>(string key, T value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            Set(key, value, slidingExpireTime, absoluteExpireTime);
            return Task.FromResult(0);
        }

        public virtual Task SetAsync<T>(KeyValuePair<string, T>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            return Task.WhenAll(pairs.Select(p => SetAsync(p.Key, p.Value, slidingExpireTime, absoluteExpireTime)));
        }

        public abstract void Remove(string key);

        public virtual void Remove(string[] keys)
        {
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public abstract void RemoveByParttern(string parttern);

        public void RemoveByParttern(string[] partterns)
        {
            foreach (var parttern in partterns)
            {
                RemoveByParttern(parttern);
            }
        }

        public virtual Task RemoveAsync(string key)
        {
            Remove(key);
            return Task.FromResult(0);
        }

        public virtual Task RemoveAsync(string[] keys)
        {
            return Task.WhenAll(keys.Select(RemoveAsync));
        }

        public abstract void Clear();

        public virtual Task ClearAsync()
        {
            Clear();
            return Task.FromResult(0);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                
                // TODOs: check disposing is true and dispose managed state (managed objects).
                // TODOs: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODOs: set large fields to null.

                disposedValue = true;
            }
        }

        // TODOs: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // TODOs: Dispose CacheBase
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODOs: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
