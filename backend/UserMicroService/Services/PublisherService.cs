using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

public class PublisherService
{
    private readonly IConnectionFactory _connectionFactory;

    public PublisherService(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void PublishUserPortfolioCreation(Guid userId)
    {
        var connection = _connectionFactory.CreateConnection();
        var channel = connection.CreateModel();
        
        channel.QueueDeclare(queue: "user_portfolio_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        var message = JsonConvert.SerializeObject(new { UserId = userId });
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: "user_portfolio_queue", basicProperties: null, body: body);
    }

    public void PublishUserPortfolioDeletion(Guid userId)
    {
        var connection = _connectionFactory.CreateConnection();
        var channel = connection.CreateModel();
        
        channel.QueueDeclare(queue: "user_portfolio_delete_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        var message = JsonConvert.SerializeObject(new { UserId = userId });
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: "user_portfolio_delete_queue", basicProperties: null, body: body);
    }
}
