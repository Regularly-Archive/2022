using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderController> _logger;
        private readonly ActivitySource _activitySource = new ActivitySource("OrderService");
        private readonly MyTraceInvoker _invoker;
        public OrderController(IHttpClientFactory httpClientFactory, ILogger<OrderController> logger, MyTraceInvoker invoker)
        {
            _httpClient = httpClientFactory.CreateClient("PaymentService");
            _logger = logger;
            _invoker = invoker;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderInfo orderInfo)
        {
            await _invoker.InvokeAsync();
            Request.Headers.ToList().ForEach(x =>
            {
                _logger.LogInformation($"Key={x.Key}, Value={x.Value}");
            });

            using (_activitySource.StartActivity("CheckOrder", ActivityKind.Internal, Request.Headers["x-b3-traceid"]))
            {
                if (await CheckOrder(orderInfo))
                {
                    Activity.Current?.AddEvent(new ActivityEvent("Prepare PaymentInfo..."));
                    Activity.Current?.AddTag("guid:x-request-id", Request.Headers["x-request-id"]);
                    var paymentInfo = new PaymentInfo()
                    {
                        OrderId = orderInfo.OrderId,
                        PaymentId = Guid.NewGuid().ToString("N"),
                        Remark = orderInfo.Remark,
                    };

                    using (_activitySource.StartActivity("PayOrder", ActivityKind.Internal))
                    {
                        var result = await PayOrder(paymentInfo) ? "成功" : "失败";
                        Activity.Current?.AddEvent(new ActivityEvent("Handle Payment API..."));
                        Activity.Current?.AddTag("guid:x-request-id", Request.Headers["x-request-id"]);
                        return new JsonResult(new { Msg = $"订单创建{result}" });
                    }
                }
                else
                {
                    return new JsonResult(new { Msg = $"订单{orderInfo.OrderId}无效" });
                }
            }
        }

        private Task<bool> CheckOrder(OrderInfo orderInfo)
        {
            return Task.FromResult(true);
        }

        private async Task<bool> PayOrder(PaymentInfo paymentInfo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(paymentInfo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/Payment", content);

            var json = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(json);

            return response.IsSuccessStatusCode;
        }
    }
}
