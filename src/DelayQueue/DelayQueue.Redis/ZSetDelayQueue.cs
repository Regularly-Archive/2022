using DelayQueue.Shared;
using CSRedis;
using Newtonsoft.Json;

namespace DelayQueue.Redis
{
    public class ZSetDelayQueue<T> where T : class
    {
        private readonly CSRedisClient _redisClient;
        private const string QueueName = "DelayQueue";

        public ZSetDelayQueue(CSRedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        public Task Enqueue(T item, TimeSpan delay)
        {
            var score = new DateTimeOffset(DateTime.UtcNow.Add(delay)).ToUnixTimeSeconds();
            _redisClient.ZAdd(QueueName, (score, JsonConvert.SerializeObject(item)));
            return Task.CompletedTask;
        }

        public async Task<T> Dequeue()
        {
            var score = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(); ;
            var records = _redisClient.ZRangeByScore(QueueName, 0, score, 1);
            if (records.Count() > 0)
            {
                var item = JsonConvert.DeserializeObject<T>(records[0]);
                await _redisClient.ZRemAsync(QueueName, item);
                return item;
            }

            return null;
        }


        public bool IsEmpty()
        {

            var count = _redisClient.ZCount(QueueName, 0, decimal.MaxValue);
            return count == 0;
        }
    }
}