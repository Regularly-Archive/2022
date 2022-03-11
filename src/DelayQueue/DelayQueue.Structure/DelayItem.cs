using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayQueue.Structure
{
    public class DelayItem<T> : IDelayItem
    {
        public T Value { get; private set; }

        public long TimeStamp { get; private set; }

        public DelayItem(T value, TimeSpan delay)
        {
            Value = value;
            TimeStamp = new DateTimeOffset(DateTime.UtcNow.Add(delay)).ToUnixTimeSeconds();
        }

        public int CompareTo(object? obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj is DelayItem<T> other)
            {
                return TimeStamp.CompareTo(other.TimeStamp);
            }

            throw new ArgumentException($"The type of object is not {nameof(DelayItem<T>)}");
        }
    }
}
