using DataServiceLayer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            // Only allow your frontend running on port 5173
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = "Host=localhost;Database=imdb;Username=postgres;Password=12345";
}

// Register services
builder.Services.AddScoped<IDataService>(provider => new DataService(connectionString));
builder.Services.AddDbContext<ImdbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Enable CORS - MUST come before UseRouting
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Custom authentication middleware (simplified version)
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    // Skip authentication for login and register endpoints
    if (path?.Contains("/users/login") == true ||
        path?.Contains("/users/register") == true)
    {
        await next();
        return;
    }

    var userName = context.Request.Headers.Authorization.FirstOrDefault();

    if (!string.IsNullOrEmpty(userName))
    {
        var dataService = context.RequestServices.GetRequiredService<IDataService>();
        var user = dataService.GetUser(userName);
        if (user != null)
        {
            context.Items["User"] = user;
        }
    }

    await next();
});

app.MapControllers();

app.Urls.Add("http://localhost:7000");

app.Run();

public partial class Program { }