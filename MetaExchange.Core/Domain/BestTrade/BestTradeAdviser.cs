using MetaExchange.Core.Domain.BestTrade.Model;
using MetaExchange.Core.Domain.Exchange;
using Microsoft.Extensions.Logging;

namespace MetaExchange.Core.Domain.BestTrade;

/// <summary>
/// The best trade adviser is the main component of MetaExchange's core functionality.
/// It analyzes the order books of all exchanges and outputs a set of orders to execute against them
/// in order to buy/sell the specified amount of cryptocurrency at the lowest/highest possible price.
/// </summary>
public class BestTradeAdviser
{
    private readonly Dictionary<string, Exchange.Model.Exchange> _exchangesById = new();
    private readonly ILogger<BestTradeAdviser> _logger;

    public BestTradeAdviser(ILogger<BestTradeAdviser> logger)
    {
        _logger = logger;
    }
    
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
        var availableEuroByExchangeId = _exchangesById
            .Values
            .Select(exchange => exchange)
            .ToDictionary(
                exchange => exchange.Id,
                exchange => exchange.AvailableFunds.Euro);
        
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
            //var exchange =  _exchangesById[orderDetail.ExchangeId];
            var availableEuroOnExchange= availableEuroByExchangeId[orderDetail.ExchangeId];
            if (priceToPay > availableEuroOnExchange)
            {
                // not enough EUR on this exchange, so we can only buy as much as we have EUR
                _logger.LogInformation("Not enough EUR on exchange {OrderDetailExchangeId} to buy {AmountToBuy} crypto at {OrderPrice} (i.e. {OrderDetailPricePerCryptoUnit} EUR/BTC).",
                    orderDetail.ExchangeId,
                    amountToBuy,
                    orderDetail.Order.Price,
                    orderDetail.PricePerCryptoUnit);
                
                priceToPay = availableEuroOnExchange;
                amountToBuy = priceToPay / orderDetail.PricePerCryptoUnit;
            }

            if (amountToBuy > 0)
            {
                _logger.LogInformation("Buying {AmountToBuy} crypto at {OrderPrice} (i.e. {OrderDetailPricePerCryptoUnit} EUR/BTC) on exchange {OrderDetailExchangeId}", amountToBuy, orderDetail.Order.Price, orderDetail.PricePerCryptoUnit, orderDetail.ExchangeId);
            }

            remainingAmountToBuy -= amountToBuy;
            availableEuroByExchangeId[orderDetail.ExchangeId] -= priceToPay;
        }
        
        if (remainingAmountToBuy > 0)
        {
            _logger.LogInformation("Could not buy the full amount of {CryptoAmount} crypto. Remaining amount to buy: {RemainingAmountToBuy}", cryptoAmount, remainingAmountToBuy);
        }
        else
        {
            _logger.LogInformation("Bought the full amount of crypto successfully.");
        }
    }
}