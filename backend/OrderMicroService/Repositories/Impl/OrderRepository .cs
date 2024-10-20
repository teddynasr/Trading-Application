using Microsoft.EntityFrameworkCore;
using OrderMicroService.Data;
using OrderMicroService.Models;

namespace OrderMicroService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDBContext _context;

        public OrderRepository(OrderDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(Guid userId)
        {
            return await _context.Orders
                .Where(order => order.UserID == userId)
                .ToListAsync();
        }

        public async Task<Order> GetOrderById(Guid orderId)
        {
            return await _context.Orders.FindAsync(orderId);
        }

        public async Task CreateOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task DeleteOrder(Guid orderId)
        {
            var order = await GetOrderById(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
