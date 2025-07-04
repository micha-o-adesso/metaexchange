using MetaExchange.Core.Domain.BestTrade;
using MetaExchange.Core.Domain.Exchange.Model;
using MetaExchange.Tests.Mock;
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
        BestTradeAdviser bestTradeAdviser = new BestTradeAdviser(_loggerFactory);
        var bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Buy, 1m);
        Assert.That(bestTrade.IsFullAmountTraded, Is.False);
        
        MockExchangeDataProvider exchangeDataProvider = new MockExchangeDataProvider([]);
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Buy, 1m);
        Assert.That(bestTrade.IsFullAmountTraded, Is.False);

        try
        {
            bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Sell, -1m);
            Assert.Fail();
        }
        catch (ArgumentException ex)
        {
            Assert.That(ex.ParamName, Is.EqualTo("cryptoAmount"));
        }
    }
    
    [Test]
    public void TestAdviserWithSingleExchange()
    {
        BestTradeAdviser bestTradeAdviser = new BestTradeAdviser(_loggerFactory);
        MockExchangeDataProvider exchangeDataProvider = new MockExchangeDataProvider([
            MockDataCreator.CreateFakeExchange(
                "exchange1", 10m, 100000m,
                new List<Tuple<decimal, decimal>>
                {
                    // buy orders
                    Tuple.Create(1m, 49000m),
                    Tuple.Create(1m, 50000m),
                    Tuple.Create(1m, 48000m),
                    
                    // sell orders
                    Tuple.Create(-1m, 53000m),
                    Tuple.Create(-1m, 51000m),
                    Tuple.Create(-1m, 52000m)
                })
        ]);
        
        // try to buy 1 crypto unit
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        var bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Buy, 1m);
        Assert.That(bestTrade.IsFullAmountTraded, Is.True);
        Assert.That(bestTrade.TotalAmountTraded, Is.EqualTo(1m));
        Assert.That(bestTrade.TotalPriceTraded, Is.EqualTo(51000m));
        Assert.That(bestTrade.AveragePricePerUnit, Is.EqualTo(51000m));
        Assert.That(bestTrade.RecommendedOrders.Count, Is.EqualTo(1));
        Assert.That(bestTrade.RecommendedOrders[0].ExchangeId, Is.EqualTo("exchange1"));
        Assert.That(bestTrade.RecommendedOrders[0].Type, Is.EqualTo(OrderType.Buy));
        Assert.That(bestTrade.RecommendedOrders[0].CryptoAmount, Is.EqualTo(1m));
        Assert.That(bestTrade.RecommendedOrders[0].PricePerCryptoUnit, Is.EqualTo(51000m));
        
        // try to sell 1 crypto unit
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Sell, 1m);
        Assert.That(bestTrade.IsFullAmountTraded, Is.True);
        Assert.That(bestTrade.TotalAmountTraded, Is.EqualTo(1m));
        Assert.That(bestTrade.TotalPriceTraded, Is.EqualTo(50000m));
        Assert.That(bestTrade.AveragePricePerUnit, Is.EqualTo(50000m));
        Assert.That(bestTrade.RecommendedOrders.Count, Is.EqualTo(1));
        Assert.That(bestTrade.RecommendedOrders[0].ExchangeId, Is.EqualTo("exchange1"));
        Assert.That(bestTrade.RecommendedOrders[0].Type, Is.EqualTo(OrderType.Sell));
        Assert.That(bestTrade.RecommendedOrders[0].CryptoAmount, Is.EqualTo(1m));
        Assert.That(bestTrade.RecommendedOrders[0].PricePerCryptoUnit, Is.EqualTo(50000m));
        
        // try to buy more than available crypto
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Buy, 4m);
        Assert.That(bestTrade.IsFullAmountTraded, Is.False);
        
        // try to sell more than available crypto
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Sell, 4m);
        Assert.That(bestTrade.IsFullAmountTraded, Is.False);
        
        // try to buy 0.5 crypto unit
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Buy, 0.5m);
        Assert.That(bestTrade.IsFullAmountTraded, Is.True);
        Assert.That(bestTrade.TotalAmountTraded, Is.EqualTo(0.5m));
        Assert.That(bestTrade.TotalPriceTraded, Is.EqualTo(25500m));
        Assert.That(bestTrade.AveragePricePerUnit, Is.EqualTo(51000m));
        Assert.That(bestTrade.RecommendedOrders.Count, Is.EqualTo(1));
        Assert.That(bestTrade.RecommendedOrders[0].ExchangeId, Is.EqualTo("exchange1"));
        Assert.That(bestTrade.RecommendedOrders[0].Type, Is.EqualTo(OrderType.Buy));
        Assert.That(bestTrade.RecommendedOrders[0].CryptoAmount, Is.EqualTo(0.5m));
        Assert.That(bestTrade.RecommendedOrders[0].PricePerCryptoUnit, Is.EqualTo(51000m));
    }

    [Test]
    public void TestAdviserWithTwoExchanges()
    {
        BestTradeAdviser bestTradeAdviser = new BestTradeAdviser(_loggerFactory);
        MockExchangeDataProvider exchangeDataProvider = new MockExchangeDataProvider([
            MockDataCreator.CreateFakeExchange(
                "exchange1", 0m, 100000m,
                new List<Tuple<decimal, decimal>>
                {
                    // sell orders
                    Tuple.Create(-0.5m, 54000m),
                    Tuple.Create(-0.5m, 50000m),
                    Tuple.Create(-0.5m, 52000m)
                }),
            MockDataCreator.CreateFakeExchange(
                "exchange2", 0m, 12750m,
                new List<Tuple<decimal, decimal>>
                {
                    // sell orders
                    Tuple.Create(-1m, 55000m),
                    Tuple.Create(-1m, 51000m),
                    Tuple.Create(-1m, 53000m)
                })
        ]);

        // try to buy 1 crypto unit -> best trade should match three orders from both exchanges
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        var bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Buy, 1m);
        Assert.That(bestTrade.IsFullAmountTraded, Is.True);
        Assert.That(bestTrade.TotalAmountTraded, Is.EqualTo(1m));
        Assert.That(bestTrade.RecommendedOrders.Count, Is.EqualTo(3));
        
        // first order should match cheepest order from exchange1, only restricted by the order's amount
        Assert.That(bestTrade.RecommendedOrders[0].ExchangeId, Is.EqualTo("exchange1"));
        Assert.That(bestTrade.RecommendedOrders[0].Type, Is.EqualTo(OrderType.Buy));
        Assert.That(bestTrade.RecommendedOrders[0].CryptoAmount, Is.EqualTo(0.5m));
        Assert.That(bestTrade.RecommendedOrders[0].PricePerCryptoUnit, Is.EqualTo(50000m));
        
        // second order should match cheepest order from exchange2, only restricted by the available funds on this exchange
        Assert.That(bestTrade.RecommendedOrders[1].ExchangeId, Is.EqualTo("exchange2"));
        Assert.That(bestTrade.RecommendedOrders[1].Type, Is.EqualTo(OrderType.Buy));
        Assert.That(bestTrade.RecommendedOrders[1].CryptoAmount, Is.EqualTo(0.25m));
        Assert.That(bestTrade.RecommendedOrders[1].PricePerCryptoUnit, Is.EqualTo(51000m));
        
        // third order should match the remaining amount on exchange1
        Assert.That(bestTrade.RecommendedOrders[2].ExchangeId, Is.EqualTo("exchange1"));
        Assert.That(bestTrade.RecommendedOrders[2].Type, Is.EqualTo(OrderType.Buy));
        Assert.That(bestTrade.RecommendedOrders[2].CryptoAmount, Is.EqualTo(0.25m));
        Assert.That(bestTrade.RecommendedOrders[2].PricePerCryptoUnit, Is.EqualTo(52000m));
    }
}