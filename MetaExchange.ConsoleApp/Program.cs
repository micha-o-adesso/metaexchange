using System.Text.Json;
using System.Text.Json.Serialization;
using Cocona;
using MetaExchange.Core.Domain.BestTrade;
using MetaExchange.Core.Domain.Exchange.Model;
using MetaExchange.Core.Infrastructure.FileExchangeDataProvider;
using Microsoft.Extensions.Logging;

CoconaApp.Run((OrderType orderType, decimal cryptoAmount, string rootFolderPath) =>
{
    using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder
        .AddFilter(l => l > LogLevel.Information)
        .AddConsole());
    
    var exchangeDataProvider = new FileExchangeDataProvider(rootFolderPath, loggerFactory);
    var bestTradeAdviser = new BestTradeAdviser(loggerFactory);
    
    bestTradeAdviser.LoadExchanges(exchangeDataProvider);
    
    var bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(orderType, cryptoAmount);
    
    Console.WriteLine(JsonSerializer.Serialize(bestTrade, new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    }));
});