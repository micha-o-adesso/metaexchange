using MetaExchange.Core.Domain.Exchange.Model;

namespace MetaExchange.Core.Domain.BestTrade.Model;

public class OrderRecommendation
{
    /// <summary>
    /// The type of the order (e.g. "Buy", "Sell").
    /// </summary>
    public required OrderType Type { get; init; }

    /// <summary>
    /// The amount of cryptocurrency to buy or sell (e.g. 0.01).
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    /// The price at which to buy or sell the cryptocurrency (e.g. 57226.46).
    /// </summary>
    public required decimal Price { get; init; }
    
    /// <summary>
    /// The unique identifier of the exchange where the order should be placed (e.g. "exchange-01").
    /// </summary>
    public required string ExchangeId { get; init; }
}