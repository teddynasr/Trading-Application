using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using OrderMicroService.Models;

namespace OrderMicroService.Services{
    public class PublisherService
    {
        private readonly IConnectionFactory _connectionFactory;

        public PublisherService(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void PublishOrder(Order order)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            
            // Declare a queue for sending messages (if it doesn't exist)
            channel.QueueDeclare(queue: "orders",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            // Serialize the order object to JSON
            var messageBody = JsonSerializer.Serialize(order);
            var body = Encoding.UTF8.GetBytes(messageBody);

            // Publish the message to the queue
            channel.BasicPublish(exchange: "",
                                routingKey: "orders",
                                basicProperties: null,
                                body: body);
        }
    }
}

