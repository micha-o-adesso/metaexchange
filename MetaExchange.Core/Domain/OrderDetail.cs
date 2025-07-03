namespace MetaExchange.Core.Domain;

public class OrderDetail
{
    public OrderDetail(string exchangeId, Order order)
    {
        ExchangeId = exchangeId;
        Order = order;

        PricePerCryptoUnit = order.Price / order.Amount;
    }
    
    /// <summary>
    /// The identifier of the exchange, e.g. "exchange-01".
    /// </summary>
    public string ExchangeId { get; private set; }
    
    /// <summary>
    /// The order.
    /// </summary>
    public Order Order { get; private set; }
    
    /// <summary>
    /// The price per cryptocurrency unit in the order.
    /// Example: if the order is to buy 0.5 BTC at a price of 57226.08 EUR, then PricePerCrypto will be 114452.16 EUR.
    /// </summary>
    public decimal PricePerCryptoUnit { get; private set; }
}