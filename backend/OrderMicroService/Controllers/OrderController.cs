using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderMicroService.Models;
using OrderMicroService.Services;

namespace OrderMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserId(Guid userId)
        {
            var orders = await _orderService.GetOrdersByUserId(userId);
            return Ok(orders);
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(Guid orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody] Order order)
        {
            await _orderService.CreateOrder(order);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.OrderID }, order);
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult> DeleteOrder(Guid orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound();
            }
            
            await _orderService.DeleteOrder(orderId);
            return NoContent();
        }
    }
}
