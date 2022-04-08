using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

            Request.Headers.ToList().ForEach(x =>
            {
                _logger.LogInformation($"Key={x.Key}, Value={x.Value}");
            });

            // 调用 EchoService
            var request = new EchoService.HelloRequest() { Name = "PaymentService" };
            _client.SayHello(request);

            return new JsonResult(new { Msg = $"支付成功, 流水号：{requestId}" });
        }
    }
}
