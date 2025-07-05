using MetaExchange.Domain.Modules.Exchange;
using MetaExchange.Domain.Modules.Exchange.Model;

namespace MetaExchange.Tests.Mock;

/// <summary>
/// A mock implementation of the IExchangeDataProvider interface for testing purposes.
/// </summary>
public class MockExchangeDataProvider : IExchangeDataProvider
{
    private readonly IEnumerable<Exchange> _exchanges;
    
    public MockExchangeDataProvider(IEnumerable<Exchange> exchanges)
    {
        _exchanges = exchanges;
    }
    
    public IEnumerable<Exchange> GetExchanges()
    {
        return _exchanges;
    }
}