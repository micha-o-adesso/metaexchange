namespace MetaExchange.Core.Domain.Exchange.Model;

/// <summary>
/// The type of the order (i.e. Buy or Sell).
/// </summary>
public enum OrderType
{
    /// <summary>
    /// A buy order is an order to purchase a security at a specified price or better.
    /// </summary>
    Buy,
    
    /// <summary>
    /// A sell order is an order to sell a security at a specified price or better.
    /// </summary>
    Sell
}