namespace MetaExchange.Core.ExchangeDataProvider.File.Model;

/// <summary>
/// The order book represents the current state of buy and sell orders on an exchange.
/// </summary>
public class OrderBook
{
    /// <summary>
    /// The buy orders in the order book.
    /// </summary>
    public List<OrderContainer> Bids { get; set; } = [];
    
    /// <summary>
    /// The sell orders in the order book.
    /// </summary>
    public List<OrderContainer> Asks { get; set; } = [];
}