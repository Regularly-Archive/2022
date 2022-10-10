using DomainEvents.Domains;
using DomainEvents.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace DomainEvents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderInfoController : ControllerBase
    {
        private readonly ILogger<OrderInfoController> _logger;

        private readonly IRepository<OrderInfo, Guid> _repository;

        private readonly ChinookContext _chinookContext;

        public OrderInfoController(ILogger<OrderInfoController> logger, IRepository<OrderInfo,Guid> repository, ChinookContext chinookContext)
        {
            _logger = logger;
            _repository = repository;
            _chinookContext = chinookContext;
        }

        [HttpPost("CreateOrder")]
        public async Task CreateOrder(OrderInfoCreateDTO order)
        {
            var orderInfo = new OrderInfo(order.Address, order.Telephone, order.Quantity, order.Remark);
            orderInfo.Confirm();

            _repository.Insert(orderInfo);

            orderInfo.ModifyAddress("陕西省西安市雁塔区卜蜂莲花超市");

            await _chinookContext.SaveChangesAsync();
        }
    }
}
