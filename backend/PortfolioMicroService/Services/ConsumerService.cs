using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using OrderMicroService.Models;
using PortfolioMicroService.Models;

namespace PortfolioMicroService.Services
{
    public class ConsumerService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ConsumerService(IConnectionFactory connectionFactory, IServiceScopeFactory serviceScopeFactory)
        {
            _connectionFactory = connectionFactory;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void StartConsuming()
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "orders", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "user_portfolio_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "user_portfolio_delete_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var orderConsumer = new EventingBasicConsumer(channel);
            var userPortfolioConsumer = new EventingBasicConsumer(channel);
            var userPortfolioDeleteConsumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "orders", autoAck: true, consumer: orderConsumer);
            channel.BasicConsume(queue: "user_portfolio_queue", autoAck: true, consumer: userPortfolioConsumer);
            channel.BasicConsume(queue: "user_portfolio_delete_queue", autoAck: true, consumer: userPortfolioDeleteConsumer);

            orderConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonSerializer.Deserialize<Order>(message);
                await HandleOrderAsync(order);
            };

            userPortfolioConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                var userPortfolioMessage = JsonSerializer.Deserialize<Dictionary<string, Guid>>(message);
                var userId = userPortfolioMessage["UserId"];
                
                await HandleUserPortfolioAsync(userId);
            };

            userPortfolioDeleteConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                var userPortfolioMessage = JsonSerializer.Deserialize<Dictionary<string, Guid>>(message);
                var userId = userPortfolioMessage["UserId"];
                
                await HandleUserPortfolioDeleteAsync(userId);
            };
        }

        private async Task HandleOrderAsync(Order order)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var positionService = scope.ServiceProvider.GetRequiredService<PositionService>();
                var userPortfolioService = scope.ServiceProvider.GetRequiredService<UserPortfolioService>();

                var existingPosition = await positionService.GetByCurrencyPairAndUserIdAsync(order.CurrencyPairID, order.UserID);
                var userPortfolio = await userPortfolioService.GetUserPortfolioByUserIdAsync(order.UserID);
                
                if (existingPosition != null)
                {
                    if (order.OrderType.Equals("buy", StringComparison.OrdinalIgnoreCase))
                    {
                        existingPosition.Amount += order.OrderAmount;
                        existingPosition.CurrentValueUSD += order.OrderUnitPrice * order.OrderAmount;

                        userPortfolio.AvailableWalletBalance -= order.OrderUnitPrice * order.OrderAmount;
                        userPortfolio.InvestedBalance += order.OrderUnitPrice * order.OrderAmount;
                    }
                    else if (order.OrderType.Equals("sell", StringComparison.OrdinalIgnoreCase))
                    {
                        existingPosition.Amount -= order.OrderAmount;
                        existingPosition.CurrentValueUSD -= order.OrderUnitPrice * order.OrderAmount;

                        userPortfolio.AvailableWalletBalance += order.OrderUnitPrice * order.OrderAmount;
                        userPortfolio.InvestedBalance -= order.OrderUnitPrice * order.OrderAmount;

                        if (existingPosition.Amount <= 0)
                        {
                            await positionService.DeletePositionAsync(existingPosition.PositionID);
                            return;
                        }
                    }

                    existingPosition.UpdatedAt = DateTime.UtcNow;
                    await positionService.UpdatePositionAsync(existingPosition);
                }
                else
                {
                    var newPosition = new Position
                    {
                        PortfolioID = userPortfolio.PortfolioID,
                        CurrencyPairID = order.CurrencyPairID,
                        Amount = order.OrderAmount,
                        CurrentValueUSD = order.OrderUnitPrice * order.OrderAmount,
                        UpdatedAt = DateTime.UtcNow
                    };

                    userPortfolio.AvailableWalletBalance -= order.OrderUnitPrice * order.OrderAmount;
                    userPortfolio.InvestedBalance += order.OrderUnitPrice * order.OrderAmount;

                    await positionService.AddPositionAsync(newPosition);
                }
                
                await userPortfolioService.UpdateUserPortfolioAsync(userPortfolio);
            }
        }

        private async Task HandleUserPortfolioAsync(Guid userId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userPortfolioService = scope.ServiceProvider.GetRequiredService<UserPortfolioService>();

                var newPortfolio = new UserPortfolio
                {
                    UserID = userId,
                    AvailableWalletBalance = 0m,
                    InvestedBalance = 0m
                };

                await userPortfolioService.AddUserPortfolioAsync(newPortfolio);
            }
        }

        private async Task HandleUserPortfolioDeleteAsync(Guid userId){
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userPortfolioService = scope.ServiceProvider.GetRequiredService<UserPortfolioService>();

                await userPortfolioService.DeleteUserPortfolioAsync(userId);
            }
        }
    }
}
