using System.Text.Json.Serialization;

namespace MetaExchange.Core.Infrastructure.FileExchangeDataProvider.Model;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderKind
{
    Limit
}