using MetaExchange.Core;
using MetaExchange.Core.ExchangeDataProvider.File;

Console.WriteLine("Hello, World!");

var exchangeDataProvider = new FileExchangeDataProvider(@"C:\VisualStudio\stuttgart\aufgabenstellung\exchanges");
var orderAdviser = new OrderAdviser();

orderAdviser.LoadExchanges(exchangeDataProvider);
