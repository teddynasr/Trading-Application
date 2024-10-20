using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text;
using PortfolioMicroService.Data;
using PortfolioMicroService.Repositories;
using PortfolioMicroService.Services;
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

// Register HttpClient
builder.Services.AddHttpClient();

// Register the repository
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IUserPortfolioRepository, UserPortfolioRepository>();

// Register the service
builder.Services.AddScoped<PositionService>();
builder.Services.AddScoped<UserPortfolioService>();
builder.Services.AddScoped<CommonServices>();
builder.Services.AddSingleton<ConsumerService>();

// Configure DB Connection
builder.Services.AddDbContext<PortfolioDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PortfolioDbConnection")));

// Connect to Redis
var multiplexer = ConnectionMultiplexer.Connect("localhost");
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddSingleton<IDatabase>(multiplexer.GetDatabase());
builder.Services.AddControllers();

// Configure RabbitMQ connection using the default port
builder.Services.AddSingleton<IConnectionFactory>(provider => new ConnectionFactory{HostName = "localhost"});

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
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"]
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

var consumerService = app.Services.GetRequiredService<ConsumerService>();
consumerService.StartConsuming();

app.MapControllers();
app.UseExceptionHandler("/error");

app.Run();
