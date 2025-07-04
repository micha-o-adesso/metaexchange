using MetaExchange.Core.Domain.BestTrade;
using MetaExchange.Core.Domain.BestTrade.Model;
using MetaExchange.Core.Domain.Exchange.Model;
using MetaExchange.Core.Infrastructure.FileExchangeDataProvider;

namespace MetaExchange.WebService.Handlers;

public class BestTradeHandler
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly string _rootFolderPath;
    
    public BestTradeHandler(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _loggerFactory = loggerFactory;
        _rootFolderPath = configuration["RootFolderPathOfExchanges"] ?? "./exchanges";
    }
    
    public BestTrade TradeCryptoAtBestPrice(OrderType tradeType, decimal cryptoAmount)
    {
        // load exchange data and calculate the best trade
        var exchangeDataProvider = new FileExchangeDataProvider(_rootFolderPath, _loggerFactory);
        var bestTradeAdviser = new BestTradeAdviser(_loggerFactory);
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        return bestTradeAdviser.TradeCryptoAtBestPrice(tradeType, cryptoAmount);
    }
}