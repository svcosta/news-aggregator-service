using Acme.NewsAggregator.Application.Interfaces;
using Acme.NewsAggregator.Infrastructure.Persistence;
using Acme.NewsAggregator.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache(); // <--- This is crucial

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Depency Injection


builder.Services.AddHttpClient<INewsAggregatorService, NewsAggregatorService>((sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["HackerNews:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<INewsAggregatorRepository, NewsAggregatorRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
