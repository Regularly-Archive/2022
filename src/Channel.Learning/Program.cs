using BenchmarkDotNet.Running;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ChannelLearning
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var flow = new ChannelDataFlow();
            await flow.Run(@"D:\项目\hugo-blog\content\posts\");
            Console.ReadKey();
        }
    }
}
