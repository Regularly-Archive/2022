using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ChannelLearning
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    [RPlotExporter]
    public class TestContext
    {
        public const int Count = 10000;

        [Benchmark(Baseline = false, Description = "BlockingCollection", OperationsPerInvoke = 1)]
        public async Task ByBlockingCollection()
        {
            var bc = new BlockingCollection<int>();

            var producer = Task.Run(() =>
            {
                for (var i = 0; i < Count; i++)
                {
                    bc.Add(i);
                    Console.WriteLine("[BlockingCollection]Producer Write Item: {0}", i);
                }
                bc.CompleteAdding();
            });

            var consumer = Task.Run(() =>
            {
                while (!bc.IsCompleted)
                {
                    if (bc.TryTake(out var item))
                    {
                        Console.WriteLine("[BlockingCollection]Consumer Read Item: {0}", item);
                    }
                }
            });

            await Task.WhenAll(producer, consumer);
        }

        [Benchmark(Baseline = false, Description = "BufferBlock", OperationsPerInvoke = 1)]
        public async Task ByBufferBlock()
        {
            var bb = new BufferBlock<int>();

            var producer = Task.Run(async () =>
            {
                for (var i = 0; i < Count; i++)
                {
                    await bb.SendAsync(i);
                    Console.WriteLine("[BufferBlock]Producer Write Item: {0}", i);
                };

                bb.Complete();
            });

            var consumer = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var item = await bb.ReceiveAsync();
                        Console.WriteLine("[BufferBlock]Consumer Read Item: {0}", item);
                    }
                    catch (Exception ex)
                    {
                        break;
                    }

                }
            });

            await Task.WhenAll(producer, consumer);
        }

        [Benchmark(Baseline = false, Description = "Channe", OperationsPerInvoke = 1)]
        public async Task ByChannel()
        {
            var channel = Channel.CreateUnbounded<int>();

            var producer = Task.Run(async () =>
            {
                for (var i = 0; i < Count; i++)
                {
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine("[Channel]Producer Write Item: {0}", i);
                };

                channel.Writer.Complete();
            });

            var consumer = Task.Run(async () =>
            {
                while (await channel.Reader.WaitToReadAsync())
                {
                    while (channel.Reader.TryRead(out var item))
                    {
                        Console.WriteLine("[Channel]Consumer Read Item: {0}", item);
                    }
                }
            });

            await Task.WhenAll(producer, consumer);
        }
    }
}
