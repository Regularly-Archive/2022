using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly EchoService.Greeter.GreeterClient _client;
        public PaymentController(ILogger<PaymentController> logger, EchoService.Greeter.GreeterClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpPost]
        public IActionResult Post([FromBody] PaymentInfo paymentInfo)
        {
            var requestId = Request.Headers["x-request-id"].ToString();
            _logger.LogInformation($"x-request-id: {requestId}");

            if (string.IsNullOrEmpty(paymentInfo.OrderId))
                return new JsonResult(new { Msg = "订单Id不允许为空" });

            // 设置请求头
            var headers = new Metadata();
            headers.Add("x-request-id", Request.Headers["x-request-id"].ToString());
            headers.Add("x-b3-traceid", Request.Headers["x-b3-traceid"].ToString());
            headers.Add("x-b3-spanid", Request.Headers["x-b3-spanid"].ToString());
            headers.Add("x-b3-parentspanid", Request.Headers["x-b3-parentspanid"].ToString());
            headers.Add("x-b3-sampled", Request.Headers["x-b3-sampled"].ToString());
            headers.Add("x-b3-flags", Request.Headers["x-b3-flags"].ToString());
            headers.Add("x-ot-span-context", Request.Headers["x-ot-span-context"].ToString());

            // 调用 EchoService
            var request = new EchoService.HelloRequest() { Name = "PaymentService" };
            _client.SayHello(request, headers);

            return new JsonResult(new { Msg = $"支付成功, 流水号：{requestId}" });
        }
    }
}
