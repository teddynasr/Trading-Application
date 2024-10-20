Simplified Architecture:

User Micro Service:
Handles basic user registration, login, and session management using JWT.
Communicates with the Order Service and Portfolio Service for user-specific data.

Market Data Micro Service:
Fetches real-time rates from an external API.
Caches rates in Redis to improve performance.
Exposes APIs to return real-time data to the frontend.

Order Micro Service:
Accepts simple buy/sell orders for a currencies, crypto and stocks.
Updates the order history and sends data to the Portfolio Service.

Portfolio Micro Service:
Tracks open positions and calculates profit/loss based on real-time rates.
Shows basic summaries of the user’s active trades.

Technologies to Focus On:
.NET 6: Use .NET 6 to implement the services. Focus on creating clean, modular APIs for each component.
RabbitMQ: Messaging bus between services.
Redis: Cache real-time forex rates to reduce API calls.
PostgreSQL: Use PostgreSQL for persistent storage of user data, order histories, and portfolio positions.
