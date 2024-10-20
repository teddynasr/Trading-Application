using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text;
using OrderMicroService.Data;
using OrderMicroService.Repositories;
using OrderMicroService.Services;
using System.Security.Claims;
using Newtonsoft.Json;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Register the repository
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register the service
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PublisherService>();

// Configure DB Connection
builder.Services.AddDbContext<OrderDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDBConnection")));

// Connect to Redis
var multiplexer = ConnectionMultiplexer.Connect("localhost");
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddSingleton<IDatabase>(multiplexer.GetDatabase());
builder.Services.AddControllers();

// Configure RabbitMQ connection
builder.Services.AddSingleton<IConnectionFactory>(provider => new ConnectionFactory
{
    HostName = "localhost" // Default RabbitMQ host
});

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Validate issuer
        ValidateAudience = true, // Validate audience
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Set valid issuer
        ValidAudience = builder.Configuration["Jwt:Audience"] // Set valid audience
    };
});

builder.Services.AddAuthorization();

builder.Services.AddLogging();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseExceptionHandler("/error");

app.Run();
