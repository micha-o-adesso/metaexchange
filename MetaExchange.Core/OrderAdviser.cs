using MetaExchange.Core.Domain;
using MetaExchange.Core.ExchangeDataProvider;

namespace MetaExchange.Core;

/// <summary>
/// The order adviser is the main component of MetaExchange's core functionality. 
/// </summary>
public class OrderAdviser
{
    private readonly List<Exchange> _exchanges = [];
    
    public void LoadExchanges(IExchangeDataProvider exchangeDataProvider)
    {
        _exchanges.Clear();
        _exchanges.AddRange(exchangeDataProvider.GetExchanges());
    }
}