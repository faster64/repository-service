using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Utilities
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content) where T : class
        {
            var contentAsString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(contentAsString);
        }
    }
}
