using MetaExchange.Core.Domain.Exchange.Model;

namespace MetaExchange.Core.ExchangeDataProvider;

/// <summary>
/// The interface for exchange data providers.
/// </summary>
public interface IExchangeDataProvider
{
    /// <summary>
    /// Gets the data of all exchanges. 
    /// </summary>
    /// <returns></returns>
    IEnumerable<Exchange> GetExchanges();
}