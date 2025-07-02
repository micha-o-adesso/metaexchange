namespace MetaExchange.Core.Model;

/// <summary>
/// The order book represents the current state of buy and sell orders on an exchange.
/// </summary>
public class OrderBook
{
    /// <summary>
    /// The buy orders in the order book.
    /// </summary>
    public List<Order> Bids { get; private set; } = [];
    
    /// <summary>
    /// The sell orders in the order book.
    /// </summary>
    public List<Order> Asks { get; private set; } = [];
}