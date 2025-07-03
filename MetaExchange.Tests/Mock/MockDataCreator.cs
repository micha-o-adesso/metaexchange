using MetaExchange.Core.Domain.Exchange.Model;

namespace MetaExchange.Tests.Mock;

public static class MockDataCreator
{
    public static Exchange CreateFakeExchange(
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
}