using Grpc.Core;
using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcStream.Services
{
    public class HeartBeatServicecs : SharedEntities.HeartBeatService.HeartBeatServiceBase
    {
        public override Task<PingReply> SimplePing(PingRequest request, ServerCallContext context)
        {
            return Task.FromResult(new PingReply() { RequestId = request.RequestId, Message = "OK" });
        }

        public override async Task<PingReply> ClientStreamPing(IAsyncStreamReader<PingRequest> requestStream, ServerCallContext context)
        {
            var requestQueue = new Queue<string>();
            while (await requestStream.MoveNext())
            {
                requestQueue.Enqueue(requestStream.Current.RequestId);
            }

            if (requestQueue.TryDequeue(out var requestId))
            {
                return new PingReply() { RequestId = requestId, Message = "OK" };
            }

            return new PingReply() { RequestId = string.Empty, Message = "" };
        }

        public override Task ServerStreamPing(PingRequest request, IServerStreamWriter<PingReply> responseStream, ServerCallContext context)
        {
            responseStream.WriteAsync(new PingReply() { RequestId = request.RequestId, Message = "OK" });
            return Task.CompletedTask;
        }

        public override async Task BothStreamPing(IAsyncStreamReader<PingRequest> requestStream, IServerStreamWriter<PingReply> responseStream, ServerCallContext context)
        {
            var requestQueue = new Queue<string>();
            while (await requestStream.MoveNext())
            {
                requestQueue.Enqueue(requestStream.Current.RequestId);
            }

            while (requestQueue.TryDequeue(out var requestId))
            {
                await responseStream.WriteAsync(new PingReply() { RequestId = requestId, Message = "OK" });
            }
        }
    }
}
