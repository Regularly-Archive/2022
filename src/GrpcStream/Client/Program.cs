using Grpc.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using SharedEntities;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Google.Protobuf;

namespace GrpcStream
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            Console.WriteLine("请输入功能选项：\r\n1. 普通 gRPC\r\n2. 客户端流式 gRPC\r\n3. 服务端流式 gRPC\r\n4. 双向流流式 gRPC\r\n5. 文件上传\r\n6. 文件下载\r\n\r\n按Q退出");

            var services = new ServiceCollection();
            services.AddGrpcClient<HeartBeatService.HeartBeatServiceClient>(client => client.Address = new Uri("http://localhost:5000"));
            services.AddGrpcClient<FileService.FileServiceClient>(client =>
            {
                client.Address = new Uri("http://localhost:5000");

            });

            var serviceProvider = services.BuildServiceProvider();
            var heartBeatClient = serviceProvider.GetRequiredService<HeartBeatService.HeartBeatServiceClient>();

            var channel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions
            {
                MaxReceiveMessageSize = null, // 10 MB
                MaxSendMessageSize = null // 10 MB
            });
            var fileServiceClient = new FileService.FileServiceClient(channel);

            var input = Console.ReadKey();
            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                switch (input.Key)
                {
                    case ConsoleKey.D1:
                        var reply1 = heartBeatClient.SimplePingAsync(new PingRequest() { RequestId = GetCurrentTimeStamp().ToString() });
                        Console.WriteLine($"{JsonConvert.SerializeObject(reply1)}");
                        break;
                    case ConsoleKey.D2:
                        var callResult2 = heartBeatClient.ClientStreamPing();
                        await callResult2.RequestStream.WriteAsync(new PingRequest() { RequestId = GetCurrentTimeStamp().ToString() });
                        await callResult2.RequestStream.CompleteAsync();
                        var reply2 = await callResult2.ResponseAsync;
                        Console.WriteLine($"{JsonConvert.SerializeObject(reply2)}");
                        break;
                    case ConsoleKey.D3:
                        var reply3 = heartBeatClient.ServerStreamPing(new PingRequest() { RequestId = GetCurrentTimeStamp().ToString() });
                        Console.WriteLine($"{JsonConvert.SerializeObject(reply3)}");
                        break;
                    case ConsoleKey.D4:
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
                    case ConsoleKey.D5:
                        var uploadResult = fileServiceClient.UploadFile();

                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "ACRouge.png");

                        using (var fileStream = File.OpenRead(uploadPath))
                        {
                            var sended = 0L;
                            var totalLength = fileStream.Length;
                            var buffer = new byte[1024 * 1024];
                            while (sended < totalLength)
                            {
                                var length = await fileStream.ReadAsync(buffer);
                                sended += length;

                                var request = new UploadFileRequest() { Content = ByteString.CopyFrom(buffer), FileName = uploadPath };
                                await uploadResult.RequestStream.WriteAsync(request);

                                Console.WriteLine($"  Send {sended }/{totalLength} via gRPC Streaming...");
                            }
                        }
                        
                        await uploadResult.RequestStream.CompleteAsync();
                        var reply = await uploadResult.ResponseAsync;
                        Console.WriteLine($"  File Uploaded to /{reply.FilePath}");

                        break;
                    case ConsoleKey.D6:
                        var downloadRequest = new DownloadFileRequest() { FilePath = "228784a3-4e1f-42ab-9fe1-fa3d42278ada.png" };
                        var downloadResult = fileServiceClient.DownloadFile(downloadRequest);
                        var downloadPath = Path.Combine(Directory.GetCurrentDirectory(), downloadRequest.FilePath);
                        if (File.Exists(downloadPath)) File.Delete(downloadPath);
                        using(var fileStram = File.Open(downloadPath, FileMode.Append, FileAccess.Write))
                        {
                            var received = 0L;
                            while (await downloadResult.ResponseStream.MoveNext(CancellationToken.None))
                            {
                                var current = downloadResult.ResponseStream.Current;
                                var buffer = current.Content.ToByteArray();

                                fileStram.Seek(received, SeekOrigin.Begin);
                                await fileStram.WriteAsync(buffer);

                                received += buffer.Length;
                                received = Math.Min(received, current.TotalSize);
                                Console.WriteLine($"  Received {received}/{current.TotalSize} via gRPC Streaming...");
                            }
                        }
                        Console.WriteLine($"  File Downloaded to {downloadPath}");
                        break;
                }

                input = Console.ReadKey();
            }
        }

        static long GetCurrentTimeStamp() => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
    }
}
