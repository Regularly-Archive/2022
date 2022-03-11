using CSRedis;
using DelayQueue.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayQueue.Redis
{
    public class KeyExpirationDelayQueue : IDelayQueue
    {
        private readonly CSRedisClient _redisClient;

        private const string EXPIRED_KEYS_CHANNEL = "__keyevent@{0}__:expired";

        private readonly ILogger<KeyExpirationDelayQueue> _logger;

        public KeyExpirationDelayQueue(CSRedisClient redisClient, ILogger<KeyExpirationDelayQueue> logger)
        {
            _redisClient = redisClient;
            _logger = logger;
        }

        public Task PutJob(TimeSpan delay, Action callback)
        {
            var guid = Guid.NewGuid().ToString("N");

            // Default Database
            var channel = string.Format(EXPIRED_KEYS_CHANNEL, 0);

            _redisClient.Set(guid, string.Empty, delay);
            _redisClient.Subscribe((channel, new Action<CSRedisClient.SubscribeMessageEventArgs>(msg =>
            {
                if (msg.Body != guid) return;
                callback?.Invoke();
            })));

            _logger.LogInformation($"{DateTimeOffset.UtcNow}:Put a new delay job.");

            return Task.CompletedTask;
        }

        public Task PutJob<T>(TimeSpan delay, T jobData, Action<T> callback)
        {
            var guid = Guid.NewGuid().ToString("N");

            // Default Database
            var channel = string.Format(EXPIRED_KEYS_CHANNEL, 0);

            _redisClient.Set(guid, jobData, delay);
            _redisClient.Subscribe((channel, new Action<CSRedisClient.SubscribeMessageEventArgs>(msg =>
            {
                if (msg.Body != guid) return;
                callback?.Invoke(jobData);
            })));

            _logger.LogInformation($"{DateTimeOffset.UtcNow}:Put a new delay job.");

            return Task.CompletedTask;
        }
    }
}
