using DelayQueue.Redis;
using DelayQueue.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DelayQueue.Host
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IDelayQueue _delayQueue;

        private readonly ILogger<JobController> _logger;

        private readonly IServiceProvider _serviceProvider;

        public JobController(ILogger<JobController> logger, IDelayQueue delayQueue,IServiceProvider serviceProvider)
        {
            _logger = logger;
            _delayQueue = delayQueue;
            _serviceProvider = serviceProvider;
        }

        [HttpGet("PutJob")]
        public async Task PutJob()
        {
            await _delayQueue.PutJob(
                TimeSpan.FromSeconds(5),
                () => _logger.LogInformation($"{DateTimeOffset.UtcNow}:Hello DelayQueue.")
            );
            await _delayQueue.PutJob(
                TimeSpan.FromSeconds(10),
                new FooBar() { Foo = "Foo", Bar = "Bar" },
                x => _logger.LogInformation($"{DateTimeOffset.UtcNow}:Hello DelayQueue, {x.Foo}, {x.Bar}.")
            );
        }

        [HttpGet("PollJobByRedis")]
        public async Task PollJobByRedis()
        {
            var redisZSetDelayQueue = _serviceProvider.GetService<ZSetDelayQueue<FooBar>>();
            await redisZSetDelayQueue.Enqueue(new FooBar() { Foo = "001", Bar = "100" }, TimeSpan.FromMinutes(1));
            await redisZSetDelayQueue.Enqueue(new FooBar() { Foo = "002", Bar = "200" }, TimeSpan.FromMinutes(2));
            await redisZSetDelayQueue.Enqueue(new FooBar() { Foo = "003", Bar = "300" }, TimeSpan.FromMinutes(3));

            while (!redisZSetDelayQueue.IsEmpty())
            {
                var item = await redisZSetDelayQueue.Dequeue();
                if (item == null)
                {
                    continue;
                }
                else
                {
                    _logger.LogInformation($"{DateTimeOffset.UtcNow}:Hello DelayQueue, {item.Foo}, {item.Bar}.");
                }
            }
        }


        [HttpGet("PollJobByPriorityQueue")]
        public async Task PollJobByPriorityQueue()
        {
            var utcNow = DateTime.UtcNow;
            var queue = new PriorityQueue<FooBar, long>();
            queue.Enqueue(new FooBar() { Foo = "001", Bar = "100" }, new DateTimeOffset(utcNow.AddSeconds(5)).ToUnixTimeSeconds());
            queue.Enqueue(new FooBar() { Foo = "002", Bar = "200" }, new DateTimeOffset(utcNow.AddSeconds(10)).ToUnixTimeSeconds());
            queue.Enqueue(new FooBar() { Foo = "003", Bar = "300" }, new DateTimeOffset(utcNow.AddSeconds(15)).ToUnixTimeSeconds());

            while (queue.Count > 0)
            {
                var current = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                var flag = queue.TryPeek(out var item, out var timestamp);
                if (!flag || current < timestamp)
                {
                    continue;
                }
                else
                {
                    item = queue.Dequeue();
                    _logger.LogInformation($"{DateTimeOffset.UtcNow}:Hello DelayQueue, {item.Foo}, {item.Bar}.");
                }
            }
        }
    }
}
