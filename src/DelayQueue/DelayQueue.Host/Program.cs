using CSRedis;
using DelayQueue.Quartz;
using DelayQueue.RabbitMQ;
using DelayQueue.Redis;
using DelayQueue.Shared;
using Quartz;
using Quartz.Impl;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSingleton<IDelayQueue, QuartzDelayQueue>();
builder.Services.AddSingleton<IScheduler>(serviceProvider =>
{
    var factory = new StdSchedulerFactory();
    return factory.GetScheduler().GetAwaiter().GetResult();
});

builder.Services.AddSingleton<ConnectionFactory>(serviceProvider =>
{
    var connectionFactory = new ConnectionFactory();
    connectionFactory.HostName = "localhost";
    connectionFactory.Port = 5672;
    connectionFactory.UserName = "root";
    connectionFactory.Password = "root";
    return connectionFactory;
});

builder.Services.AddSingleton<IDelayQueue, RabbitDelayQueue>();
//builder.Services.AddSingleton<IDelayQueue, KeyExpirationDelayQueue>();
builder.Services.AddSingleton(typeof(ZSetDelayQueue<>));
builder.Services.AddSingleton<CSRedisClient>(serviceProvider =>
{
    var redisClient = new CSRedisClient("127.0.0.1:6380");
    RedisHelper.Initialization(redisClient);
    return redisClient;
});

var app = builder.Build();

var scheduler = app.Services.GetService<IScheduler>();
await scheduler.Start();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
