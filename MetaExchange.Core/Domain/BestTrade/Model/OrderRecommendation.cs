using MetaExchange.Core.Domain.Exchange.Model;

namespace MetaExchange.Core.Domain.BestTrade.Model;

/// <summary>
/// The order recommendation represents a suggested order to be executed
/// against the order book of an exchange.
/// </summary>
public class OrderRecommendation
{
    /// <summary>
    /// The type of the order (e.g. "Buy", "Sell").
    /// </summary>
    public required OrderType Type { get; init; }

    /// <summary>
    /// The amount of cryptocurrency to buy or sell (e.g. 0.01 BTC).
    /// </summary>
    public required decimal CryptoAmount { get; init; }

    /// <summary>
    /// The price per unit at which to buy or sell the cryptocurrency (e.g. 57226.46 EUR/BTC).
    /// </summary>
    public required decimal PricePerCryptoUnit { get; init; }
    
    /// <summary>
    /// The unique identifier of the exchange where the order should be placed (e.g. "exchange-01").
    /// </summary>
    public required string ExchangeId { get; init; }
}