using Google.Protobuf;
using Grpc.Core;

namespace DynamicGrpc
{
    public static class GrpcUtils
    {
        internal static Method<TRequest, TResponse> CreateMethod<TRequest, TResponse>(MethodType methodType, string serviceName, string methodName)
            where TRequest : class, IMessage, new()
            where TResponse : class, IMessage, new()
        {
            return new Method<TRequest, TResponse>(
                methodType,
                serviceName,
                methodName,
                CreateMarshaller<TRequest>(),
                CreateMarshaller<TResponse>());
        }

        private static Marshaller<TMessage> CreateMarshaller<TMessage>()
              where TMessage : class, IMessage, new()
        {
            return new Marshaller<TMessage>(
                m => m.ToByteArray(),
                d =>
                {
                    var m = new TMessage();
                    m.MergeFrom(d);
                    return m;
                });
        }
    }
}
