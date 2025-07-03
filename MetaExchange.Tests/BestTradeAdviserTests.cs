using MetaExchange.Core.Domain.BestTrade;
using MetaExchange.Core.Domain.Exchange.Model;
using Microsoft.Extensions.Logging;

namespace MetaExchange.Tests;

public class BestTradeAdviserTests
{
    private ILoggerFactory _loggerFactory; 
    
    [SetUp]
    public void Setup()
    {
        _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    }
    
    [TearDown]
    public void TearDown()
    {
        _loggerFactory.Dispose();
    }

    [Test]
    public void TestAdviserWithoutExchangeData()
    {
        BestTradeAdviser bestTradeAdviser = new BestTradeAdviser(_loggerFactory.CreateLogger<BestTradeAdviser>());
        var bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Buy, 1m);
        Assert.That(bestTrade, Is.Null);
        
        MockExchangeDataProvider exchangeDataProvider = new MockExchangeDataProvider([]);
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Buy, 1m);
        Assert.That(bestTrade, Is.Null);
    }
}