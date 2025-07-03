using MetaExchange.Core.Domain.BestTrade;
using MetaExchange.Core.Domain.Exchange.Model;
using MetaExchange.Core.Infrastructure.FileExchangeDataProvider;
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

var exchangeDataProvider = new FileExchangeDataProvider(
    @"C:\VisualStudio\stuttgart\aufgabenstellung\exchanges",
    factory.CreateLogger<FileExchangeDataProvider>());

var bestTradeAdviser = new BestTradeAdviser(factory.CreateLogger<BestTradeAdviser>());

bestTradeAdviser.LoadExchanges(exchangeDataProvider);
var bestBuy = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Buy, 10m);

bestTradeAdviser.LoadExchanges(exchangeDataProvider);
var bestSell = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Sell, 10m);

Console.WriteLine();


