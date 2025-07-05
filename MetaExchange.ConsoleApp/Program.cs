using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cocona;
using MetaExchange.Domain.Modules.BestTrade;
using MetaExchange.Domain.Modules.Exchange.Model;
using MetaExchange.Infrastructure.FileExchangeDataProvider;
using Microsoft.Extensions.Logging;

// set the current culture to invariant culture to ensure consistent parsing of decimal numbers
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

// use Cocona to create a console application which parses command line arguments
// example usage:
// MetaExchange.ConsoleApp.exe --order-type Buy --crypto-amount 0.27 --root-folder-path ..\..\..\..\ExampleData\exchanges
CoconaApp.Run((OrderType? orderType, decimal? cryptoAmount, string rootFolderPath) =>
{
    using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder
        .AddFilter(l => l > LogLevel.Information)
        .AddConsole());

    if (!orderType.HasValue)
    {
        // if the order type is not specified via command line argument, prompt the user to enter it
        Console.WriteLine($"Please specify the order type ({nameof(OrderType.Buy)} or {nameof(OrderType.Sell)}):");
        var orderTypeInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(orderTypeInput) || !Enum.TryParse<OrderType>(orderTypeInput, true, out var parsedOrderType))
        {
            Console.WriteLine($"Invalid order type specified. Please use '{nameof(OrderType.Buy)}' or '{nameof(OrderType.Sell)}'.");
            return;
        }
        orderType = parsedOrderType;
    }
    
    if (!cryptoAmount.HasValue || cryptoAmount < 0m)
    {
        // if the crypto amount is not specified via command line argument, prompt the user to enter it
        Console.WriteLine($"Please specify the crypto amount to trade:");
        var cryptoAmountInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(cryptoAmountInput) ||
            !decimal.TryParse(cryptoAmountInput, out var parsedCryptoAmount) ||
            parsedCryptoAmount < 0m)
        {
            Console.WriteLine($"Invalid crypto amount specified.");
            return;
        }
        cryptoAmount = parsedCryptoAmount;
    }
    
    // load exchange data and calculate the best trade
    var exchangeDataProvider = new FileExchangeDataProvider(rootFolderPath, loggerFactory);
    var bestTradeAdviser = new BestTradeAdviser(loggerFactory);
    bestTradeAdviser.LoadExchanges(exchangeDataProvider);
    var bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(orderType.Value, cryptoAmount.Value);
    
    // output the best trade
    Console.WriteLine("Best trade:");
    Console.WriteLine(JsonSerializer.Serialize(bestTrade, new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    }));
});