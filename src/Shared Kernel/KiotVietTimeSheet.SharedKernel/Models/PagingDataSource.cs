using System.Collections.Generic;

namespace KiotVietTimeSheet.SharedKernel.Models
{
    public class PagingDataSource<T> where T : class
    {
        public long Total { get; set; }
        public IList<T> Data { get; set; }
        public object Filters { get; set; }
        public bool? HasNoPermisstion { get; set; }
    }
}
