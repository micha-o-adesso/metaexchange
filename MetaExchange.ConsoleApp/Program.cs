using MetaExchange.Core;
using MetaExchange.Core.ExchangeDataProvider.File;
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

var exchangeDataProvider = new FileExchangeDataProvider(
    @"C:\VisualStudio\stuttgart\aufgabenstellung\exchanges",
    factory.CreateLogger<FileExchangeDataProvider>());

var orderAdviser = new OrderAdviser(factory.CreateLogger<OrderAdviser>());

orderAdviser.LoadExchanges(exchangeDataProvider);
orderAdviser.BuyCryptoAtLowestPossiblePrice(1000m);
