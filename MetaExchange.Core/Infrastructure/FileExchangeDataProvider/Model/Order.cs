namespace MetaExchange.Core.Infrastructure.FileExchangeDataProvider.Model;

/// <summary>
/// The order represents a buy or sell transaction for a cryptocurrency on an exchange.
/// </summary>
public class Order
{
    /// <summary>
    /// The unique identifier for the order (e.g. "6e9fe255-a776-4965-9bf4-9f076361f5cb").
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The date and time in UTC when the order was created (e.g. "2024-03-01T14:41:06.563Z").
    /// </summary>
    public required DateTime Time { get; set; }

    /// <summary>
    /// The type of the order (e.g. "Buy", "Sell").
    /// </summary>
    public required OrderType Type { get; set; }

    /// <summary>
    /// The kind of the order (e.g. "Market", "Limit").
    /// </summary>
    public required OrderKind Kind { get; set; }

    /// <summary>
    /// The amount of cryptocurrency to buy or sell (e.g. 0.01).
    /// </summary>
    public required decimal Amount { get; set; }

    /// <summary>
    /// The price at which to buy or sell the cryptocurrency (e.g. 57226.46).
    /// </summary>
    public required decimal Price { get; set; }
}