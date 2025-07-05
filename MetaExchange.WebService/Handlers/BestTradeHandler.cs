using MetaExchange.Core.Domain.BestTrade;
using MetaExchange.Core.Domain.Exchange;
using MetaExchange.Core.Domain.Exchange.Model;
using Microsoft.AspNetCore.Mvc;

namespace MetaExchange.WebService.Handlers;

public static class BestTradeHandler
{
    /// <summary>
    /// Analyzes the order books of all exchanges and outputs a set of orders to execute
    /// against these order books in order to buy/sell the specified amount
    /// of cryptocurrency at the lowest/highest possible price.
    /// </summary>
    /// <param name="tradeType">The type of the trade (i.e. Buy or Sell).</param>
    /// <param name="cryptoAmount">The amount of cryptocurrency to trade.</param>
    /// <param name="exchangeDataProvider"></param>
    /// <param name="bestTradeAdviser"></param>
    /// <returns></returns>
    public static IResult TradeCryptoAtBestPrice(
        OrderType tradeType,
        decimal cryptoAmount,
        [FromServices] IExchangeDataProvider exchangeDataProvider,
        [FromServices] BestTradeAdviser bestTradeAdviser)
    {
        if (cryptoAmount < 0m)
        {
            return Results.BadRequest("The crypto amount to trade must be greater than or equal to 0.");
        }
        
        // load exchange data and calculate the best trade
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        var bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(tradeType, cryptoAmount);
        return Results.Ok(bestTrade);
    }
}