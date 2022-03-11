namespace DelayQueue.Shared
{
    public interface IDelayQueue
    {
        Task PutJob(TimeSpan delay, Action callback);
        Task PutJob<T>(TimeSpan delay, T jobData, Action<T> callback);
    }
}