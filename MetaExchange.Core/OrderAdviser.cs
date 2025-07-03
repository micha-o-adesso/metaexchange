using MetaExchange.Core.Domain;
using MetaExchange.Core.ExchangeDataProvider;

namespace MetaExchange.Core;

/// <summary>
/// The order adviser is the main component of MetaExchange's core functionality. 
/// </summary>
public class OrderAdviser
{
    private readonly List<Exchange> _exchanges = [];
    
    /// <summary>
    /// Loads the data of all exchanges from the specified exchange data provider.
    /// </summary>
    /// <param name="exchangeDataProvider">The exchange data provider.</param>
    public void LoadExchanges(IExchangeDataProvider exchangeDataProvider)
    {
        _exchanges.Clear();
        _exchanges.AddRange(exchangeDataProvider.GetExchanges());
    }

    /// <summary>
    /// Outputs a set of orders to execute against the order books of all exchanges
    /// to buy the specified amount of crypto at the lowest possible price.
    /// </summary>
    /// <param name="cryptoAmount">The amount of cryptocurrency to buy.</param>
    public void BuyCryptoAtLowestPossiblePrice(decimal cryptoAmount)
    {
        var orderDetails = _exchanges
            .SelectMany(exchange => exchange.OrderBook.Asks
                .Select(order => new OrderDetail(exchange.Id, order)))
            .OrderBy(orderDetail => orderDetail.PricePerCryptoUnit)
            .ToList();
        
        decimal remainingAmountToBuy = cryptoAmount;
        foreach (var orderDetail in orderDetails)
        {
            if (remainingAmountToBuy <= 0)
                break;

            var amountToBuy = Math.Min(remainingAmountToBuy, orderDetail.Order.Amount);
            Console.WriteLine($"Buying {amountToBuy} crypto at {orderDetail.Order.Price} (i.e. {orderDetail.PricePerCryptoUnit} EUR/BTC) on exchange {orderDetail.ExchangeId}");
            remainingAmountToBuy -= amountToBuy;
        }
    }
}