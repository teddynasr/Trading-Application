using OrderMicroService.Models;
using OrderMicroService.Repositories;
using RabbitMQ.Client;

namespace OrderMicroService.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IConnectionFactory _connectionFactory;
        private readonly PublisherService _publisherService;

        public OrderService(IOrderRepository orderRepository, IConnectionFactory connectionFactory)
        {
            _orderRepository = orderRepository;
            _connectionFactory = connectionFactory;
            _publisherService = new PublisherService(_connectionFactory);
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _orderRepository.GetAllOrders();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(Guid userId)
        {
            return await _orderRepository.GetOrdersByUserId(userId);
        }

        public async Task<Order> GetOrderById(Guid orderId)
        {
            return await _orderRepository.GetOrderById(orderId);
        }

        public async Task CreateOrder(Order order)
        {
            await _orderRepository.CreateOrder(order);
            await _orderRepository.SaveChangesAsync();
            
            // Publish the order to the Portfolio service
            _publisherService.PublishOrder(order);
        }

        public async Task DeleteOrder(Guid orderId){
            await _orderRepository.DeleteOrder(orderId);
        }
    }
}
