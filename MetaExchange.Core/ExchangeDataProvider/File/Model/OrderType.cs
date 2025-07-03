using System.Text.Json.Serialization;

namespace MetaExchange.Core.ExchangeDataProvider.File.Model;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderType
{
    Buy,
    Sell
}