using Microsoft.EntityFrameworkCore;
using MarketDataMicroService.Repositories;
using MarketDataMicroService.Services;
using MarketDataMicroService.Data;
using StackExchange.Redis;

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

// Register Repositories for DI
builder.Services.AddScoped<ICurrencyPairRepository, CurrencyPairRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Register Services for DI
builder.Services.AddScoped<CurrencyPairService>();
builder.Services.AddScoped<CategoryService>();

// Register HttpClient for use in CurrencyPairService
builder.Services.AddHttpClient<CurrencyPairService>();

// Configure DB Connection
builder.Services.AddDbContext<MarketDataDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MarketDataDBConnection")));

// Connect to Redis
var multiplexer = ConnectionMultiplexer.Connect("localhost");
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddSingleton<IDatabase>(multiplexer.GetDatabase());
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseRouting();
app.MapControllers();
app.UseExceptionHandler("/error");

app.Run();
