namespace DelayQueue.Structure
{
    public interface IDelayItem : IComparable
    {
        public long TimeStamp { get; set; }
    }
}
