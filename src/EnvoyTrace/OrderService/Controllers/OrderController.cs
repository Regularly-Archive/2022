using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public OrderController(IHttpClientFactory httpClientFactory, ILogger<OrderController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PaymentService");
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderInfo orderInfo)
        {
            var paymentInfo = new PaymentInfo()
            {
                OrderId = orderInfo.OrderId,
                PaymentId = Guid.NewGuid().ToString("N"),
                Remark = orderInfo.Remark,
            };

            Request.Headers.ToList().ForEach(x =>
            {
                _logger.LogInformation($"Key={x.Key}, Value={x.Value}");
            });

            // 调用/Payment接口
            var content = new StringContent(JsonConvert.SerializeObject(paymentInfo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/Payment", content);

            var json = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(json);

            var result = response.IsSuccessStatusCode ? "成功" : "失败";
            return new JsonResult(new { Msg = $"订单创建{result}" });
        }
    }
}
