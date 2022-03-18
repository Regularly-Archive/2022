using DynamicGrpc;
using DynamicGrpc.Services;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapPost("/api/greet.Greeter/SayHello", async ctx =>
{
    using (var streamReader = new StreamReader(ctx.Request.Body))
    {
        var channel = GrpcChannel.ForAddress("https://localhost:5001");

        var sayHelloMethod = GrpcUtils.CreateMethod<HelloRequest, HelloReply>(MethodType.Unary, "greet.Greeter", "SayHello");

        var callInvoker = channel.CreateCallInvoker();

        var callOptions = new CallOptions().WithDeadline(DateTime.UtcNow.AddSeconds(30));

        var payload = await streamReader.ReadToEndAsync();
        var request = HelloRequest.Parser.ParseJson(payload);
        var reply = await callInvoker.AsyncUnaryCall<HelloRequest,HelloReply>(sayHelloMethod, string.Empty, callOptions, request);

        var response = JsonConvert.SerializeObject(reply, new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        });

        ctx.Response.StatusCode = 200;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(response));
    }
});

app.Run();
