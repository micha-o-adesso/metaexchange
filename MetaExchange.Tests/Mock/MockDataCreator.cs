using MetaExchange.Core.Domain.Exchange.Model;

namespace MetaExchange.Tests.Mock;

public static class MockDataCreator
{
    /// <summary>
    /// Creates fake exchange data for testing purposes.
    /// </summary>
    /// <param name="exchangeId">The identifier of the exchange.</param>
    /// <param name="availableCrypto">The available funds in cryptocurrency on the exchange.</param>
    /// <param name="availableEuro">The available funds in fiat currency on the exchange.</param>
    /// <param name="orderAmountsAndPrices">A list of tuples (first item is amount, second item is price).
    /// Positive amounts represent Buy orders, while negative amounts represent Sell orders.</param>
    /// <returns></returns>
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
            Type = orderAmountsAndPrices.Item1 > 0
                ? OrderType.Buy   // positive amounts represent Buy orders
                : OrderType.Sell, // negative amounts represent Sell orders
            Kind = OrderKind.Limit,
            CryptoAmount = Math.Abs(orderAmountsAndPrices.Item1),
            PricePerCryptoUnit = orderAmountsAndPrices.Item2
        };
    }
}