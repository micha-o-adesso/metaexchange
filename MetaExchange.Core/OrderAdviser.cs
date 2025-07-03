using MetaExchange.Core.Domain;
using MetaExchange.Core.ExchangeDataProvider;

namespace MetaExchange.Core;

/// <summary>
/// The order adviser is the main component of MetaExchange's core functionality. 
/// </summary>
public class OrderAdviser
{
    private readonly Dictionary<string, Exchange> _exchangesById = new(); 
    
    /// <summary>
    /// Loads the data of all exchanges from the specified exchange data provider.
    /// </summary>
    /// <param name="exchangeDataProvider">The exchange data provider.</param>
    public void LoadExchanges(IExchangeDataProvider exchangeDataProvider)
    {
        _exchangesById.Clear();
        foreach (var exchange in exchangeDataProvider.GetExchanges())
        {
            _exchangesById.Add(exchange.Id, exchange);
        }
    }

    /// <summary>
    /// Outputs a set of orders to execute against the order books of all exchanges
    /// to buy the specified amount of crypto at the lowest possible price.
    /// </summary>
    /// <param name="cryptoAmount">The amount of cryptocurrency to buy.</param>
    public void BuyCryptoAtLowestPossiblePrice(decimal cryptoAmount)
    {
        // sort all orders from all exchanges by price per crypto unit (EUR/BTC)
        var orderDetails = _exchangesById
            .Values
            .SelectMany(exchange => exchange.OrderBook.Asks
                .Select(order => new OrderDetail(exchange.Id, order)))
            .OrderBy(orderDetail => orderDetail.PricePerCryptoUnit)
            .ToList();
        
        // loop through the sorted orders and buy crypto until the specified amount is reached
        decimal remainingAmountToBuy = cryptoAmount;
        foreach (var orderDetail in orderDetails)
        {
            if (remainingAmountToBuy <= 0)
                break;

            // what amount can we buy from this order?
            var amountToBuy = Math.Min(remainingAmountToBuy, orderDetail.Order.Amount);
            
            // how much do we have to pay for this amount?
            var priceToPay = amountToBuy * orderDetail.PricePerCryptoUnit;

            // do we have enough EUR on this exchange?
            var exchange = _exchangesById[orderDetail.ExchangeId];
            if (priceToPay > exchange.AvailableFunds.Euro)
            {
                // not enough EUR on this exchange, so we can only buy as much as we have EUR
                Console.WriteLine($"Not enough EUR on exchange {orderDetail.ExchangeId} to buy {amountToBuy} crypto at {orderDetail.Order.Price} (i.e. {orderDetail.PricePerCryptoUnit} EUR/BTC).");
                
                priceToPay = exchange.AvailableFunds.Euro;
                amountToBuy = priceToPay / orderDetail.PricePerCryptoUnit;
            }

            if (amountToBuy > 0)
            {
                Console.WriteLine($"Buying {amountToBuy} crypto at {orderDetail.Order.Price} (i.e. {orderDetail.PricePerCryptoUnit} EUR/BTC) on exchange {orderDetail.ExchangeId}");
            }

            remainingAmountToBuy -= amountToBuy;
            exchange.AvailableFunds.Euro -= priceToPay;
        }
        
        if (remainingAmountToBuy > 0)
        {
            Console.WriteLine($"Could not buy the full amount of {cryptoAmount} crypto. Remaining amount to buy: {remainingAmountToBuy}");
        }
        else
        {
            Console.WriteLine("Bought the full amount of crypto successfully.");
        }
    }
}