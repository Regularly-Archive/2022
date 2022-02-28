using Grpc.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using SharedEntities;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcStream
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            Console.WriteLine("请输入功能选项：\r\n1. 普通 gRPC\r\n2. 客户端流式 gRPC\r\n3. 服务端流式 gRPC\r\n4. 双向流流式 gRPC\r\n按Q退出");

            var services = new ServiceCollection();
            services.AddGrpcClient<HeartBeatService.HeartBeatServiceClient>(client => client.Address = new Uri("http://localhost:5000"));

            var serviceProvider = services.BuildServiceProvider();
            var heartBeatClient = serviceProvider.GetRequiredService<HeartBeatService.HeartBeatServiceClient>();

            var input = Console.ReadLine();
            while (input != null && Console.ReadKey().Key != ConsoleKey.Q)
            {
                switch (input)
                {
                    case "1":
                        var reply1 = heartBeatClient.SimplePingAsync(new PingRequest() { RequestId = GetCurrentTimeStamp().ToString() });
                        Console.WriteLine($"{JsonConvert.SerializeObject(reply1)}");
                        break;
                    case "2":
                        var callResult2 = heartBeatClient.ClientStreamPing();
                        await callResult2.RequestStream.WriteAsync(new PingRequest() { RequestId = GetCurrentTimeStamp().ToString() });
                        await callResult2.RequestStream.CompleteAsync();
                        var reply2 = await callResult2.ResponseAsync;
                        Console.WriteLine($"{JsonConvert.SerializeObject(reply2)}");
                        break;
                    case "3":
                        var reply3 = heartBeatClient.ServerStreamPing(new PingRequest() { RequestId = GetCurrentTimeStamp().ToString() });
                        Console.WriteLine($"{JsonConvert.SerializeObject(reply3)}");
                        break;
                    case "4":
                        var callResult4 = heartBeatClient.BothStreamPing();

                        for (var i = 0; i < 10; i++)
                        {
                            var request = new PingRequest() { RequestId = GetCurrentTimeStamp().ToString() };
                            await callResult4.RequestStream.WriteAsync(request);
                            Console.WriteLine($"I => {JsonConvert.SerializeObject(request)}");
                            Thread.Sleep(500);
                        }

                        Console.WriteLine($"-----------------------------------------------");

                        await callResult4.RequestStream.CompleteAsync();
                        while (await callResult4.ResponseStream.MoveNext(CancellationToken.None))
                        {
                            var reply4 = callResult4.ResponseStream.Current;
                            Console.WriteLine($"O => {JsonConvert.SerializeObject(reply4)}");
                        }
                        break;
                }

                input = Console.ReadLine();
            }
        }

        static long GetCurrentTimeStamp() => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
    }
}
