namespace MetaExchange.Domain.Modules.Exchange;

/// <summary>
/// The interface for exchange data providers.
/// </summary>
public interface IExchangeDataProvider
{
    /// <summary>
    /// Gets the data of all exchanges. 
    /// </summary>
    /// <returns></returns>
    IEnumerable<Modules.Exchange.Model.Exchange> GetExchanges();
}