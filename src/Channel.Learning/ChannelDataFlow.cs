using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelLearning
{
    class ChannelDataFlow
    {
        private Task<Channel<string>> GetFiles(string root)
        {
            var filePathChannel = Channel.CreateUnbounded<string>();
            var directoryInfo = new DirectoryInfo(root);

            foreach (var file in directoryInfo.EnumerateFileSystemInfos())
            {
                filePathChannel.Writer.TryWrite(file.FullName);
            }

            filePathChannel.Writer.Complete();
            return Task.FromResult(filePathChannel);
        }

        private async Task<Channel<string>[]> Analyse(Channel<string> rootChannel)
        {
            var counterChannel = Channel.CreateUnbounded<string>();
            var errorsChannel = Channel.CreateUnbounded<string>();

            while (await rootChannel.Reader.WaitToReadAsync())
            {
                await foreach (var filePath in rootChannel.Reader.ReadAllAsync())
                {
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Extension == ".md")
                    {
                        var totalWords = File.ReadAllText(filePath).Length;
                        counterChannel.Writer.TryWrite($"文章 [{fileInfo.Name}] 共 {totalWords} 个字符.");
                    }
                    else
                    {
                        errorsChannel.Writer.TryWrite($"路径 [{filePath}] 是文件夹或者格式不正确.");
                    }
                }
            }

            counterChannel.Writer.Complete();
            errorsChannel.Writer.Complete();

            return new Channel<string>[] { counterChannel, errorsChannel };
        }

        private async Task<Channel<string>> Merge(params Channel<string>[] channels)
        {
            var mergeTasks = new List<Task>();
            var outputChannel = Channel.CreateUnbounded<string>();

            foreach (var channel in channels)
            {
                var thisChannel = channel;
                var mergeTask = Task.Run(async () =>
                {
                    while (await thisChannel.Reader.WaitToReadAsync())
                    {
                        await foreach (var item in thisChannel.Reader.ReadAllAsync())
                        {
                            outputChannel.Writer.TryWrite(item);
                        }
                    }
                });

                mergeTasks.Add(mergeTask);
            }

            await Task.WhenAll(mergeTasks);
            outputChannel.Writer.Complete();

            return outputChannel;
        }

        public async Task Run(string root)
        {
            var filePathChannel = await GetFiles(root);
            var analysedChannels = await Analyse(filePathChannel);
            var mergedChannel = await Merge(analysedChannels);
            while (await mergedChannel.Reader.WaitToReadAsync())
                await foreach (var item in mergedChannel.Reader.ReadAllAsync())
                    Console.WriteLine(item);
        }
    }
}
