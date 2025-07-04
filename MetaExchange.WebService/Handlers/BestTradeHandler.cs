using MetaExchange.Core.Domain.BestTrade;
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
    
    public IResult TradeCryptoAtBestPrice(OrderType tradeType, decimal cryptoAmount)
    {
        if (cryptoAmount < 0m)
        {
            return Results.BadRequest("The crypto amount to trade must be greater than or equal to 0.");
        }
        
        // load exchange data and calculate the best trade
        var exchangeDataProvider = new FileExchangeDataProvider(_rootFolderPath, _loggerFactory);
        var bestTradeAdviser = new BestTradeAdviser(_loggerFactory);
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        var bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(tradeType, cryptoAmount);
        return Results.Ok(bestTrade);
    }
}