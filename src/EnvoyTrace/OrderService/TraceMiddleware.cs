using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace OrderService
{
    public class TraceMiddleware
    {
        private readonly RequestDelegate _next;


        public TraceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("x-b3-traceid"))
            {
                CallContext.SetData("TraceId", context.Request.Headers["x-b3-traceid"]);
            } else
            {
                CallContext.SetData("TraceId", Guid.NewGuid().ToString("N"));
            }

            await _next(context);
        }
    }
}
