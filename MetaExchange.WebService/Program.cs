using System.Text.Json.Serialization;
using MetaExchange.Domain.Modules.BestTrade;
using MetaExchange.Domain.Modules.BestTrade.Model;
using MetaExchange.Domain.Modules.Exchange;
using MetaExchange.Infrastructure.FileExchangeDataProvider;
using MetaExchange.WebService.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// configure dependency injection
var rootFolderPath = builder.Configuration["RootFolderPathOfExchanges"] ?? "./exchanges";
builder.Services.AddSingleton<IExchangeDataProvider>(sp => new FileExchangeDataProvider(
    rootFolderPath,
    sp.GetRequiredService<ILoggerFactory>()));
builder.Services.AddSingleton<BestTradeAdviser>();

// https://stackoverflow.com/questions/76643787/how-to-make-enum-serialization-default-to-string-in-minimal-api-endpoints-and-sw
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "MetaExchange API V1");
    });
}

app.UseHttpsRedirection();

app
    .MapGet("/besttrade", BestTradeHandler.TradeCryptoAtBestPrice)
    .WithDescription("Analyzes the order books of all exchanges and outputs a set of orders to execute against these order books in order to buy/sell the specified amount of cryptocurrency at the lowest/highest possible price.")
    .Produces<BestTrade>()
    .Produces(StatusCodes.Status400BadRequest);

app.Run();