using MetaExchange.Core.Domain.BestTrade.Model;
using MetaExchange.Core.Domain.Exchange.Model;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Quic;

namespace MetaExchange.WebService.Handlers;

public class BestTradeHandler
{
    private readonly ILoggerFactory _loggerFactory;
    
    public BestTradeHandler(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }
    
    /// <summary>
    /// Trades at best price.
    /// </summary>
    /// <param name="tradeType"></param>
    /// <param name="cryptoAmount"></param>
    /// <returns></returns>
    public BestTrade TradeCryptoAtBestPrice(OrderType tradeType, decimal cryptoAmount)
    {
        return new BestTrade();
    }
}