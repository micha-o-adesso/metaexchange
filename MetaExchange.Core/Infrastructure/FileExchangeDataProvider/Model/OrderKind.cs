using System.Text.Json.Serialization;

namespace MetaExchange.Core.Infrastructure.FileExchangeDataProvider.Model;

/// <summary>
/// The kind of the order. Currently only Limit orders are allowed.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderKind
{
    /// <summary>
    /// A limit order is an order to buy or sell a security at a specified price or better.
    /// </summary>
    Limit
}