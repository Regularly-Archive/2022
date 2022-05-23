using Google.Protobuf;
using Google.Protobuf.Reflection;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Reflection.V1Alpha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicGrpc
{
    public class DynamicGrpcCallInvoker
    {
        private readonly ServerReflection.ServerReflectionClient _serverReflectionClient;

        public DynamicGrpcCallInvoker(ServerReflection.ServerReflectionClient serverReflectionClient)
        {
            _serverReflectionClient = serverReflectionClient;
        }

        public async Task Execute(string serviceUrl, MethodType methodType, string serviceName, string methodName)
        {
            var cts = new CancellationTokenSource();
            var deadline = DateTime.UtcNow.AddSeconds(30);

            var serviceNames = await ResolveListServices(deadline, cts.Token);
            if (!serviceNames.Contains(serviceName))
                throw new Exception($"Invalid ServiceName \"{serviceName}\"");

            var buffedFileProtos = await ResolveFileProtos(serviceName, deadline, cts.Token);
            var fileDescriptors = ResolveFileDescriptors(buffedFileProtos);

            var serviceDescriptors = fileDescriptors.SelectMany(x => x.Services).ToList();
            var messageDescriptors = fileDescriptors.SelectMany(x => x.MessageTypes).ToList();

            var methodDescriptors = serviceDescriptors.SelectMany(x => x.Methods).ToList();

            var channel = GrpcChannel.ForAddress(serviceUrl);
            var callInvoker = channel.CreateCallInvoker();
        }

        private async Task<List<string>> ResolveListServices(DateTime deadline, CancellationToken cancellationToken)
        {
            var callResult = _serverReflectionClient.ServerReflectionInfo(deadline: deadline, cancellationToken: cancellationToken);

            var resolveServiceListTask = Task.Run(async () =>
            {
                var serviceNames = new List<string>();
                while (await callResult.ResponseStream.MoveNext(cancellationToken))
                {
                    foreach (var service in callResult.ResponseStream.Current.ListServicesResponse.Service)
                    {
                        serviceNames.Add(service.Name);
                    }
                }
                return serviceNames;
            });

            var request = new ServerReflectionRequest() { ListServices = "" };
            await callResult.RequestStream.WriteAsync(request);
            await callResult.RequestStream.CompleteAsync();

            return await resolveServiceListTask;
        }

        private async Task<List<(ByteString Buffer, FileDescriptorProto Proto)>> ResolveFileProtos(string serviceName, DateTime deadline, CancellationToken cancellationToken)
        {
            var callResult = _serverReflectionClient.ServerReflectionInfo(deadline: deadline, cancellationToken: cancellationToken);

            var resolveFileDescriptorTask = Task.Run(async () =>
            {
                var buffedFileProtos = new List<(ByteString, FileDescriptorProto)>();

                while (await callResult.ResponseStream.MoveNext(cancellationToken))
                {
                    var buffers = callResult.ResponseStream.Current.FileDescriptorResponse.FileDescriptorProto.ToList();
                    buffers.ForEach(buffer =>
                    {
                        var proto = FileDescriptorProto.Parser.ParseFrom(buffer.ToByteArray());
                        buffedFileProtos.Add((buffer, proto));
                    });
                }

                return buffedFileProtos;
            });

            var request = new ServerReflectionRequest() { FileContainingSymbol = serviceName };
            await callResult.RequestStream.WriteAsync(request);
            await callResult.RequestStream.CompleteAsync();

            return await resolveFileDescriptorTask;
        }

        private List<FileDescriptor> ResolveFileDescriptors(List<(ByteString Buffer, FileDescriptorProto Proto)> buffedFileProtos)
        {
            var sortedProtos = new List<ByteString>();
            var loadedProtos = buffedFileProtos.GroupBy(x => x.Proto.Name).ToDictionary(x => x.Key, x => x.ToList()[0]);
            var resolvedProtos = new HashSet<string>();

            while (loadedProtos.Count() > 0)
            {
                var buffedFileProto = loadedProtos.Values.FirstOrDefault(x => x.Proto.Dependency.All(dependency => resolvedProtos.Contains(dependency)));
                if (buffedFileProto.Buffer != null && buffedFileProto.Proto != null)
                {
                    resolvedProtos.Add(buffedFileProto.Proto.Name);
                    loadedProtos.Remove(buffedFileProto.Proto.Name);
                    sortedProtos.Add(buffedFileProto.Buffer);
                }
            }

            return FileDescriptor.BuildFromByteStrings(sortedProtos).ToList();
        }
    }
}
