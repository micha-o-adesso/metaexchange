using MetaExchange.Core.Domain.BestTrade.Model;
using MetaExchange.Core.Domain.Exchange;
using MetaExchange.Core.Domain.Exchange.Model;
using Microsoft.Extensions.Logging;

namespace MetaExchange.Core.Domain.BestTrade;

/// <summary>
/// The best trade adviser is the main component of MetaExchange's core functionality.
/// It analyzes the order books of all exchanges and outputs a set of orders to
/// execute against these order books in order to buy/sell the specified
/// amount of cryptocurrency at the lowest/highest possible price.
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
    /// Outputs a set of orders to execute against these order books in order to buy/sell
    /// the specified amount of cryptocurrency at the lowest/highest possible price.
    /// </summary>
    /// <param name="tradeType">The type of the trade (i.e. Buy or Sell).</param>
    /// <param name="cryptoAmount">The amount of cryptocurrency to trade.</param>
    public Model.BestTrade TradeCryptoAtBestPrice(OrderType tradeType, decimal cryptoAmount)
    {
        Model.BestTrade bestTrade = new Model.BestTrade
        {
            RemainingAmountToTrade = cryptoAmount
        };
        
        var availableFundsByExchangeId = _exchangesById
            .Values
            .ToDictionary(
                exchange => exchange.Id,
                exchange => tradeType == OrderType.Buy
                    ? exchange.AvailableFunds.Euro     // buy -> we have to consider our EUR funds
                    : exchange.AvailableFunds.Crypto); // sell -> we have to consider our crypto funds
        
        // sort all orders from all exchanges by price per crypto unit (EUR/BTC)
        var orderDetails = _exchangesById
            .Values
            .SelectMany(exchange => (tradeType == OrderType.Buy
                    ? exchange.OrderBook.Asks  // buy -> we want to buy at the lowest price, so we look at the asks
                    : exchange.OrderBook.Bids) // sell -> we want to sell at the highest price, so we look at the bids
                .Select(order => new OrderDetail(exchange.Id, order)));
        
        var orderDetailsBestFirst = tradeType == OrderType.Buy
            ? orderDetails // buy -> we want to buy at the lowest price, so we sort by ascending price
                    .OrderBy(orderDetail => orderDetail.PricePerCryptoUnit)
                    .ToList()
            : orderDetails // sell -> we want to sell at the highest price, so we sort by descending price
                .OrderByDescending(orderDetail => orderDetail.PricePerCryptoUnit)
                .ToList();
        
        // loop through the sorted orders and trade crypto until the specified amount is reached
        //decimal remainingAmountToTrade = cryptoAmount;
        foreach (var orderDetail in orderDetailsBestFirst)
        {
            if (bestTrade.RemainingAmountToTrade <= 0)
                break;

            // what amount can we trade from this order?
            var amountToTrade = Math.Min(bestTrade.RemainingAmountToTrade, orderDetail.Order.Amount);
            
            // how much will this reduce our available funds on this exchange?
            var fundReduction= tradeType == OrderType.Buy
                ? amountToTrade * orderDetail.PricePerCryptoUnit // buy -> reduces our EUR funds
                : amountToTrade;                                 // sell -> reduces our crypto funds

            // do we have enough available funds on this exchange?
            var availableFundsOnExchange= availableFundsByExchangeId[orderDetail.ExchangeId];
            if (fundReduction > availableFundsOnExchange)
            {
                // not enough funds on this exchange, so we can only trade as much as we have
                _logger.LogInformation("Not enough EUR on exchange {OrderDetailExchangeId} to buy {AmountToBuy} crypto at {OrderPrice} (i.e. {OrderDetailPricePerCryptoUnit} EUR/BTC).",
                    orderDetail.ExchangeId,
                    amountToTrade,
                    orderDetail.Order.Price,
                    orderDetail.PricePerCryptoUnit);
                
                fundReduction = availableFundsOnExchange;
                amountToTrade = tradeType == OrderType.Buy
                    ? fundReduction / orderDetail.PricePerCryptoUnit // buy -> we can only buy as much as we can afford
                    : fundReduction;                                 // sell -> we can only sell as much as we have
            }

            if (amountToTrade > 0)
            {
                _logger.LogInformation("Buying {AmountToBuy} crypto at {OrderPrice} (i.e. {OrderDetailPricePerCryptoUnit} EUR/BTC) on exchange {OrderDetailExchangeId}", amountToTrade, orderDetail.Order.Price, orderDetail.PricePerCryptoUnit, orderDetail.ExchangeId);
                
                availableFundsByExchangeId[orderDetail.ExchangeId] -= fundReduction;

                bestTrade = new Model.BestTrade
                {
                    RecommendedOrders = bestTrade.RecommendedOrders
                        .Append(new OrderRecommendation
                        {
                            Type = tradeType,
                            Price = orderDetail.PricePerCryptoUnit,
                            Amount = amountToTrade,
                            ExchangeId = orderDetail.ExchangeId
                        })
                        .ToList(),
                    TotalAmount = bestTrade.TotalAmount + amountToTrade,
                    TotalPrice = bestTrade.TotalPrice + amountToTrade * orderDetail.PricePerCryptoUnit,
                    RemainingAmountToTrade = bestTrade.RemainingAmountToTrade - amountToTrade
                };
            }
        }
        
        if (bestTrade.RemainingAmountToTrade > 0)
        {
            _logger.LogInformation("Could not buy the full amount of {CryptoAmount} crypto. Remaining amount to buy: {RemainingAmountToBuy}", cryptoAmount, bestTrade.RemainingAmountToTrade);
            
        }
        else
        {
            _logger.LogInformation("Bought the full amount of crypto successfully.");
        }

        return bestTrade;
    }
}