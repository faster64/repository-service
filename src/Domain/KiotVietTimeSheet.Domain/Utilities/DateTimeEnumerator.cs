using System;
using System.Collections;

namespace KiotVietTimeSheet.Domain.Utilities
{
    public class DateTimeEnumerator : IEnumerable
    {
        private readonly DateTime _begin;
        private readonly DateTime _end;
        private readonly int _freq;

        public DateTimeEnumerator(DateTime begin, DateTime end, int freq = 1)
        {
            _begin = begin;
            _end = end;
            _freq = freq;
        }
        public IEnumerator GetEnumerator()
        {
            for (var date = _begin; date <= _end; date = date.AddDays(_freq))
            {
                yield return date;
            }
        }
    }
}
