namespace DelayQueue.Structure
{
    public class DelayQueue<T> : IDelayQueue<T> where T : IDelayItem
    {
        private readonly SortedList<long, T> _sortedList = new SortedList<long, T>();

        public T Dequeue()
        {
            return default(T);
        }

        public void Enqueue(T item)
        {
            _sortedList.Add(item.TimeStamp, item);
        }
    }
}