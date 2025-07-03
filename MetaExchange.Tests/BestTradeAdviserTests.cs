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
    
    [Test]
    public void TestAdviserWithSingleExchange()
    {
        BestTradeAdviser bestTradeAdviser = new BestTradeAdviser(_loggerFactory.CreateLogger<BestTradeAdviser>());
        MockExchangeDataProvider exchangeDataProvider = new MockExchangeDataProvider([
            CreateFakeExchange(
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
        Assert.That(bestTrade, Is.Not.Null);
        Assert.That(bestTrade.TotalAmount, Is.EqualTo(1m));
        Assert.That(bestTrade.TotalPrice, Is.EqualTo(51000m));
        Assert.That(bestTrade.AveragePricePerUnit, Is.EqualTo(51000m));
        Assert.That(bestTrade.RecommendedOrders.Count, Is.EqualTo(1));
        Assert.That(bestTrade.RecommendedOrders[0].ExchangeId, Is.EqualTo("exchange1"));
        Assert.That(bestTrade.RecommendedOrders[0].Type, Is.EqualTo(OrderType.Buy));
        Assert.That(bestTrade.RecommendedOrders[0].Amount, Is.EqualTo(1m));
        Assert.That(bestTrade.RecommendedOrders[0].Price, Is.EqualTo(51000m));
        
        // try to sell 1 crypto unit
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Sell, 1m);
        Assert.That(bestTrade, Is.Not.Null);
        Assert.That(bestTrade.TotalAmount, Is.EqualTo(1m));
        Assert.That(bestTrade.TotalPrice, Is.EqualTo(50000m));
        Assert.That(bestTrade.AveragePricePerUnit, Is.EqualTo(50000m));
        Assert.That(bestTrade.RecommendedOrders.Count, Is.EqualTo(1));
        Assert.That(bestTrade.RecommendedOrders[0].ExchangeId, Is.EqualTo("exchange1"));
        Assert.That(bestTrade.RecommendedOrders[0].Type, Is.EqualTo(OrderType.Sell));
        Assert.That(bestTrade.RecommendedOrders[0].Amount, Is.EqualTo(1m));
        Assert.That(bestTrade.RecommendedOrders[0].Price, Is.EqualTo(50000m));
        
        // try to buy more than available crypto
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Buy, 4m);
        Assert.That(bestTrade, Is.Null);
        
        // try to sell more than available crypto
        bestTradeAdviser.LoadExchanges(exchangeDataProvider);
        bestTrade = bestTradeAdviser.TradeCryptoAtBestPrice(OrderType.Sell, 4m);
        Assert.That(bestTrade, Is.Null);
    }

    #region Fake data creation

    private static Exchange CreateFakeExchange(
        string exchangeId,
        decimal availableCrypto,
        decimal availableEuro,
        IList<Tuple<decimal, decimal>> orderAmountsAndPrices)
    {
        return new Exchange
        {
            Id = exchangeId,
            AvailableFunds = new()
            {
                Crypto = availableCrypto,
                Euro = availableEuro
            },
            OrderBook = new()
            {
                Bids = orderAmountsAndPrices
                    .Select(CreateFakeOrder)
                    .Where(order => order.Type == OrderType.Buy)
                    .ToList(),
                
                Asks = orderAmountsAndPrices
                    .Select(CreateFakeOrder)
                    .Where(order => order.Type == OrderType.Sell)
                    .ToList()
            }
        };
    }

    private static Order CreateFakeOrder(Tuple<decimal, decimal> orderAmountsAndPrices)
    {
        return new Order
        {
            Id = Guid.NewGuid().ToString(),
            Time = DateTime.UtcNow,
            Type = orderAmountsAndPrices.Item1 > 0 ? OrderType.Buy : OrderType.Sell,
            Kind = OrderKind.Limit,
            Amount = Math.Abs(orderAmountsAndPrices.Item1),
            Price = orderAmountsAndPrices.Item2
        };
    }
    
    #endregion
}