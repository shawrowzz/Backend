using DataServiceLayer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    
    connectionString = "Host=localhost;Database=imdb;Username=postgres;Password=12345";
}


builder.Services.AddScoped<IDataService>(provider => new DataService(connectionString));


builder.Services.AddDbContext<ImdbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();


app.Use(async (context, next) =>
{
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