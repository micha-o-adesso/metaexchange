using MetaExchange.Core.Domain.Exchange.Model;

namespace MetaExchange.Core.Domain.BestTrade.Model;

public class OrderDetail
{
    public OrderDetail(string exchangeId, Order order)
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