using MetaExchange.Core.Domain.Exchange.Model;

namespace MetaExchange.Core.Domain.BestTrade.Model;

/// <summary>
/// The order detail represents an order on a specific exchange.
/// </summary>
public class ExchangeOrder
{
    public ExchangeOrder(string exchangeId, Order order)
    {
        ExchangeId = exchangeId;
        Order = order;
    }
    
    /// <summary>
    /// The identifier of the exchange, e.g. "exchange-01".
    /// </summary>
    public string ExchangeId { get; private set; }
    
    /// <summary>
    /// The order.
    /// </summary>
    public Order Order { get; private set; }
}