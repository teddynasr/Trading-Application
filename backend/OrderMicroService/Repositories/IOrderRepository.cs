using OrderMicroService.Models;

namespace OrderMicroService.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task<IEnumerable<Order>> GetOrdersByUserId(Guid userId);
        Task<Order> GetOrderById(Guid orderId);
        Task CreateOrder(Order order);
        Task DeleteOrder(Guid orderId);
        Task SaveChangesAsync();
    }
}
