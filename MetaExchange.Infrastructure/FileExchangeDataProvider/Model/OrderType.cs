using System.Text.Json.Serialization;

namespace MetaExchange.Infrastructure.FileExchangeDataProvider.Model;

/// <summary>
/// The type of the order (i.e. Buy or Sell).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
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