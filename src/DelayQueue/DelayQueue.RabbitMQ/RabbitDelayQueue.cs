using DelayQueue.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace DelayQueue.RabbitMQ
{
    public class RabbitDelayQueue : IDelayQueue
    {
        private const string DELAY_EXCHANGE_NAME = "{0}.delay.exchange";

        private const string DELAY_QUEUE_NAME = "{0}.delay.queue";

        private const string DL_EXCHANGE_NAME = "{0}.deadletter.exchange";

        private const string DL_QUEUE_NAME = "{0}.deadletter.queue";

        private const string ROUTING_KEY_NORMAL = "normal.routingKey";

        private const string ROUTING_KEY_DEAD = "dead.routingKey";

        private readonly ConnectionFactory _connectionFactory;

        private readonly IConnection _connection;

        private readonly ILogger<RabbitDelayQueue> _logger;

        private EventingBasicConsumer _basicConsumer;

        private IModel _consumerChannel;

        public RabbitDelayQueue(ConnectionFactory connectionFactory, ILogger<RabbitDelayQueue> logger)
        {
            _connectionFactory = connectionFactory;
            _connection = _connectionFactory.CreateConnection();
            _logger = logger;
        }

        public async Task PutJob(TimeSpan delay, Action callback)
        {
            using (var channel = _connection.CreateModel())
            {
                // 普通交换机 order.delay.exchnage
                var exchangeNormal = string.Format(DELAY_EXCHANGE_NAME, "default");
                channel.ExchangeDeclare(exchangeNormal, "direct", true, false, null);


                // 普通队列
                var queueNormal = string.Format(DELAY_QUEUE_NAME, "default");
                var arguments = new Dictionary<string, object>
                {
                    ["x-message-ttl"] = 5000,
                    ["x-dead-letter-exchange"] = string.Format(DL_EXCHANGE_NAME, "default"),
                    ["x-dead-letter-routing-key"] = ROUTING_KEY_DEAD
                };
                channel.QueueDeclare(queue: queueNormal, true, false, false, arguments: arguments);

                // 绑定交换器
                channel.QueueBind(queueNormal, exchangeNormal, ROUTING_KEY_NORMAL);


                // 发送消息
                for (var i = 0; i < 10; i++)
                {
                    var body = Encoding.UTF8.GetBytes("Hello RabbitMQ");
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;
                    channel.BasicPublish(exchange: exchangeNormal, routingKey: ROUTING_KEY_NORMAL, mandatory: true, basicProperties: properties, body: body);
                    Thread.Sleep(1000);
                }
            }

            _consumerChannel = _connection.CreateModel();
            await StartConsume(_consumerChannel, null);
        }

        public async Task PutJob<T>(TimeSpan delay, T jobData, Action<T> callback)
        {
            using (var channel = _connection.CreateModel())
            {
                // 普通交换机 order.delay.exchnage
                var exchangeNormal = string.Format(DELAY_EXCHANGE_NAME, "default");
                channel.ExchangeDeclare(exchangeNormal, "direct", true, false, null);


                // 普通队列
                var queueNormal = string.Format(DELAY_QUEUE_NAME, "default");
                var arguments = new Dictionary<string, object>
                {
                    ["x-message-ttl"] = 5000,
                    ["x-dead-letter-exchange"] = string.Format(DL_EXCHANGE_NAME, "default"),
                    ["x-dead-letter-routing-key"] = ROUTING_KEY_DEAD
                };
                channel.QueueDeclare(queue: queueNormal, true, false, false, arguments: arguments);

                // 绑定交换器
                channel.QueueBind(queueNormal, exchangeNormal, ROUTING_KEY_NORMAL);


                // 发送消息
                for (var i = 0; i < 10; i++)
                {
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jobData));
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;
                    channel.BasicPublish(exchange: exchangeNormal, routingKey: ROUTING_KEY_NORMAL, mandatory: true, basicProperties: properties, body: body);
                    Thread.Sleep(1000);
                }
            }

            _consumerChannel = _connection.CreateModel();
            await StartConsume(_consumerChannel, body =>
             {
                 jobData = JsonConvert.DeserializeObject<T>(body);
                 callback?.Invoke(jobData);

             });
        }

        private Task StartConsume(IModel channel, Action<string> bodyAction)
        {
            // 死信交换机 order.deadletter.exchange
            var exchangeDead = string.Format(DL_EXCHANGE_NAME, "default");
            _consumerChannel.ExchangeDeclare(exchangeDead, "direct", true, false, null);

            // 死信队列 order.deadletter.queue
            var queueDead = string.Format(DL_QUEUE_NAME, "default");
            _consumerChannel.QueueDeclare(queue: queueDead, true, false, false, null);

            _consumerChannel.QueueBind(queueDead, exchangeDead, ROUTING_KEY_DEAD);

            if (_basicConsumer == null)
            {
                _basicConsumer = new EventingBasicConsumer(_consumerChannel);
                _consumerChannel.BasicConsume(queue: queueDead, autoAck: false, consumer: _basicConsumer);
                _basicConsumer.Received += (s, e) =>
                {
                    var body = Encoding.UTF8.GetString(e.Body.ToArray());
                    bodyAction?.Invoke(body);
                    //_consumerChannel.BasicAck(e.DeliveryTag, false);
                };
            }

            return Task.CompletedTask;
        }
    }
}