namespace MetaExchange.Domain.Modules.Exchange.Model;

/// <summary>
/// The exchange represents a trading platform where users can buy and sell cryptocurrencies.
/// </summary>
public class Exchange
{
    /// <summary>
    /// The identifier of the exchange, e.g. "exchange-01".
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// The available funds on the exchange.
    /// </summary>
    public required AvailableFunds AvailableFunds { get; init; }
    
    /// <summary>
    /// The order book for the exchange, which contains the current buy and sell orders.
    /// </summary>
    public required OrderBook OrderBook { get; init; }
}